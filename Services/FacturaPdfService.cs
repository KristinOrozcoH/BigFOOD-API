using API_BigFOOD.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MailKit.Net.Smtp;
using MimeKit;

namespace API_BigFOOD.Services
{
    public class FacturaPdfService
    {
        private readonly DbContextBigFOOD _context;
        private readonly ITipoCambioService _tipoCambioService;

        public FacturaPdfService(DbContextBigFOOD context, ITipoCambioService tipoCambioService)
        {
            _context = context;
            _tipoCambioService = tipoCambioService;
        }

        public async Task<string> GenerarYEnviarFacturaPDF(int numeroFactura)
        {
            var factura = _context.Facturas.FirstOrDefault(f => f.numero == numeroFactura);
            if (factura == null)
                return $"No existe la factura #{numeroFactura}";

            var detalles = _context.Det_Facturas.Where(d => d.numFactura == numeroFactura).ToList();
            var cliente = _context.Clientes.FirstOrDefault(c => c.cedulaLegal == factura.codCliente);
            var tipoCambio = await _tipoCambioService.ObtenerTipoCambioDolarAsync();

            if (cliente == null || string.IsNullOrEmpty(cliente.Email))
                return "No se puede enviar la factura: el cliente no tiene correo electrónico válido.";
            if (tipoCambio == null || tipoCambio.venta <= 0)
                return "No se puede generar PDF: no se pudo obtener el tipo de cambio actual.";

            decimal totalDolares = factura.Total / tipoCambio.venta;

            string nombreArchivo = $"Factura_{numeroFactura}.pdf";
            string rutaArchivo = Path.Combine(Path.GetTempPath(), nombreArchivo);

            using (FileStream fs = new FileStream(rutaArchivo, FileMode.Create))
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, fs);
                doc.Open();

                var azul = new BaseColor(0, 70, 160);
                var gris = new BaseColor(240, 240, 240);
                var negritaAzul = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, azul);

                doc.Add(new Paragraph("BigFOOD - Factura Electrónica", negritaAzul));
                doc.Add(new Paragraph($"Factura #{factura.numero}"));
                doc.Add(new Paragraph($"Fecha: {factura.Fecha:dd/MM/yyyy}"));
                doc.Add(new Paragraph($"Cliente: {cliente.NombreCompleto}"));
                doc.Add(new Paragraph($"Correo: {cliente.Email}"));
                doc.Add(new Paragraph(" "));

                PdfPTable tabla = new PdfPTable(6);
                tabla.WidthPercentage = 100;
                tabla.SetWidths(new float[] { 15f, 30f, 10f, 15f, 15f, 15f });

                void AgregarCeldaEncabezado(string texto)
                {
                    var celda = new PdfPCell(new Phrase(texto, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, azul)))
                    {
                        BackgroundColor = gris,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    tabla.AddCell(celda);
                }

                AgregarCeldaEncabezado("Código");
                AgregarCeldaEncabezado("Producto");
                AgregarCeldaEncabezado("Cantidad");
                AgregarCeldaEncabezado("Precio U.");
                AgregarCeldaEncabezado("Descuento");
                AgregarCeldaEncabezado("Impuesto");

                foreach (var item in detalles)
                {
                    var producto = _context.Productos.FirstOrDefault(p => p.CodigoInterno == item.codInterno);

                    tabla.AddCell(item.codInterno.ToString());
                    tabla.AddCell(producto?.Descripcion ?? "Producto no encontrado");
                    tabla.AddCell(item.cantidad.ToString());
                    tabla.AddCell(item.PrecioUnitario.ToString("N2"));
                    tabla.AddCell($"{item.PorDescuento}%");
                    tabla.AddCell($"{item.PorImp}%");
                }

                doc.Add(tabla);
                doc.Add(new Paragraph(" "));

                void AgregarTotal(string label, decimal valor, Font? font = null)
                {
                    font ??= FontFactory.GetFont(FontFactory.HELVETICA, 11);
                    var p = new Paragraph($"{label}: {valor:N2}", font)
                    {
                        Alignment = Element.ALIGN_RIGHT
                    };
                    doc.Add(p);
                }

                AgregarTotal("Subtotal", factura.Subtotal);
                AgregarTotal("Descuento", factura.MontoDescuento);
                AgregarTotal("Impuesto", factura.MontoImpuesto);
                AgregarTotal("TOTAL (CRC)", factura.Total, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.Green));
                AgregarTotal($"Tipo de cambio aplicado: ₡{tipoCambio.venta:N2}", 0, FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.DarkGray));
                AgregarTotal("TOTAL (USD)", totalDolares, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.Blue));

                doc.Add(new Paragraph("\nGracias por su compra - BigFOOD", FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 9, BaseColor.DarkGray)));
                doc.Close();
            }

            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("BigFOOD App", "bigfoodservices.sa@gmail.com"));
            mensaje.To.Add(MailboxAddress.Parse(cliente.Email));
            mensaje.Subject = $"Factura #{numeroFactura} - BigFOOD";

            var builder = new BodyBuilder
            {
                TextBody = "Adjunto encontrará la factura de su compra en BigFOOD."
            };
            builder.Attachments.Add(rutaArchivo);
            mensaje.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("bigfoodservices.sa@gmail.com", "extrfslxzjdpbocp");
            await smtp.SendAsync(mensaje);
            await smtp.DisconnectAsync(true);

            return "Factura generada y enviada correctamente.";
        }
    }
}

