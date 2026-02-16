using EventHub.Core.Entities;
using EventHub.Core.Interfaces.Services;
using EventHub.Core.Repositories;
using EventHub.Core.DTOs.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QRCoder;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using System.IO;
using EventHub.Infrastructure.Data;
//using iText.BouncyCastle.Adapters;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Properties;
using iText;
using System.IO;
using System.Threading.Tasks;

namespace EventHub.Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly EventHubDbContext _context;

        public TicketService(ITicketRepository ticketRepository, EventHubDbContext context)
        {
            _ticketRepository = ticketRepository;
            _context = context;

        }

        public async Task<IEnumerable<TicketReadDto>> GetAllAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();
            return tickets.Select(t => new TicketReadDto
            {
                Id = t.Id,
                EventId = t.EventId,
                UserId = t.UserId,
                Price = t.Price,
                SeatNumber = t.SeatNumber,
                CreatedAt = t.CreatedAt
            });

        }


        public async Task<TicketReadDto?> GetByIdAsync(int id)
        {
            var t = await _ticketRepository.GetByIdAsync(id);
            if (t == null) return null;

            return new TicketReadDto
            {
                Id = t.Id,
                EventId = t.EventId,
                UserId = t.UserId,
                
                Price = t.Price,
                SeatNumber = t.SeatNumber,
                CreatedAt = t.CreatedAt
            };
        }

        public async Task<IEnumerable<TicketReadDto>> GetByUserIdAsync(int userId)
        {
            var tickets = await _ticketRepository.GetByUserAsync(userId); 
            return tickets.Select(t => new TicketReadDto
            {
                Id = t.Id,
                EventId = t.EventId,
                UserId = t.UserId,
                
                Price = t.Price,
                SeatNumber = t.SeatNumber,
                CreatedAt = t.CreatedAt
            });
        }

        public async Task<TicketReadDto> CreateAsync(TicketCreateDto dto)
        {
            
            var ticket = new Ticket
            {
                EventId = dto.EventId,
                UserId = dto.UserId,
                RegistrationId = dto.RegistrationId,
                Price = dto.Price,
                
                SeatNumber = dto.SeatNumber,
                CreatedAt = DateTime.UtcNow
            };

            await _ticketRepository.AddAsync(ticket); 
            await _context.SaveChangesAsync();
            var qrBytes = GenerateQrCode($"Ticket-{ticket.Id}-User-{ticket.UserId}");

            ticket.PdfPath = GeneratePdf(ticket, qrBytes);

            await _ticketRepository.UpdateAsync(ticket);

            return new TicketReadDto
            {
                Id = ticket.Id,
                EventId = ticket.EventId,
                UserId = ticket.UserId,
                RegistrationId = ticket.RegistrationId,
                Price = ticket.Price,
                SeatNumber = ticket.SeatNumber,
                CreatedAt = ticket.CreatedAt,
                PdfPath = $"/{ticket.PdfPath.Replace("\\", "/")}" 
            };
        }


        public async Task<TicketReadDto?> UpdateAsync(TicketUpdateDto dto)
        {
            var ticket = await _ticketRepository.GetByIdAsync(dto.Id);
            if (ticket == null) return null;

            ticket.Price = dto.Price;
            ticket.SeatNumber = dto.SeatNumber;

            await _ticketRepository.UpdateAsync(ticket);

            return new TicketReadDto
            {
                Id = ticket.Id,
                EventId = ticket.EventId,
                UserId = ticket.UserId,
               PdfPath = ticket.PdfPath,
                Price = ticket.Price,
                SeatNumber = ticket.SeatNumber,
                CreatedAt = ticket.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null) return false;

            await _ticketRepository.DeleteAsync(ticket.Id);
            return true;
        }
        private byte[] GenerateQrCode(string text)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }



        /* private string GeneratePdf(Ticket ticket, byte[] qrBytes)
         {
             var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tickets");
             Directory.CreateDirectory(folder);

             var filePath = Path.Combine(folder, $"ticket_{ticket.Id}.pdf");

             using var writer = new PdfWriter(filePath);
             using var pdf = new PdfDocument(writer);
             var document = new Document(pdf);

             document.Add(new Paragraph($"🎫 Ticket ID: {ticket.Id}"));
             document.Add(new Paragraph($"📅 Event ID: {ticket.EventId}"));
             document.Add(new Paragraph($"👤 User ID: {ticket.UserId}"));
             document.Add(new Paragraph($"💺 Seat Number: {ticket.SeatNumber}"));
             document.Add(new Paragraph($"💵 Price: {ticket.Price} JOD"));

             var qrImage = new Image(ImageDataFactory.Create(qrBytes));
             qrImage.ScaleAbsolute(100, 100);
             document.Add(qrImage);

             document.Close();
             return $"/tickets/ticket_{ticket.Id}.pdf";

             //return Path.Combine("tickets", $"ticket_{ticket.Id}.pdf");
         }  */
        private string GeneratePdf(Ticket ticket, byte[] qrBytes)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tickets");
            Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, $"ticket_{ticket.Id}.pdf");

            using var writer = new PdfWriter(filePath, new WriterProperties().SetCompressionLevel(0));
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            //  document.Add(new Paragraph($"Ticket ID: {ticket.Id}"));
            //  document.Add(new Paragraph($"Event ID: {ticket.EventId}"));
         

            
            var eventEntity = _context.Set<EEvent>().FirstOrDefault(e => e.Id == ticket.EventId);
            document.Add(new Paragraph($"Event:  {eventEntity?.Title ?? "Unknown"}"));
            document.Add(new Paragraph($"Full Name:  { ticket.Attendee.FullName}"));
            document.Add(new Paragraph($"Role - :  { ticket.Attendee.Role}"));

            document.Add(new Paragraph($"🎫 Ticket ID: {ticket.Id}"));
            document.Add(new Paragraph($"📅 Event ID: {ticket.EventId}"));
            document.Add(new Paragraph($"👤 User ID: {ticket.UserId}"));
            document.Add(new Paragraph($"💺 Seat Number: {ticket.SeatNumber}"));
            document.Add(new Paragraph($"💵 Price: {ticket.Price} JOD"));

            var qrImage = new Image(ImageDataFactory.Create(qrBytes));
            qrImage.ScaleAbsolute(100, 100);
            document.Add(qrImage);

            document.Close();
            return $"/tickets/ticket_{ticket.Id}.pdf";
        }

    }
}
    
