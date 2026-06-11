using EventHub.Core.DTOs.Ticket;
using EventHub.Core.Entities;
using EventHub.Core.Exceptions;
using EventHub.Core.Interfaces.Services;
using EventHub.Core.Repositories;
using EventHub.Infrastructure.Data;
using iText;
using iText.IO.Image;
using iText.Kernel.Pdf;
//using iText.BouncyCastle.Adapters;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace EventHub.Infrastructure.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<IEnumerable<TicketReadDto>> GetAllAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();
            return tickets.Select(MapToDto);
        }

        public async Task<TicketReadDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Invalid ticket ID");

            var ticket = await _ticketRepository.GetByIdAsync(id);
            return ticket == null ? null : MapToDto(ticket);
        }

        public async Task<IEnumerable<TicketReadDto>> GetByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ValidationException("Invalid user ID");

            var tickets = await _ticketRepository.GetByUserAsync(userId);
            return tickets.Select(MapToDto);
        }

        public async Task<TicketReadDto> CreateAsync(TicketCreateDto dto)
        {
            if (dto == null)
                throw new ValidationException("Ticket data is required");

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

            var ticketWithDetails = await _ticketRepository.GetByIdAsync(ticket.Id);
            if (ticketWithDetails == null)
                throw new NotFoundException("Ticket not found after creation");

            var qrBytes = GenerateQrCode($"Ticket-{ticket.Id}-User-{ticket.UserId}");
            ticket.PdfPath = GeneratePdf(ticketWithDetails, qrBytes);

            await _ticketRepository.UpdateAsync(ticket);

            return MapToDto(ticketWithDetails) with
            {
                PdfPath = $"/{ticket.PdfPath?.Replace("\\", "/")}"
            };
        }

        public async Task<TicketReadDto> UpdateAsync(int id, TicketUpdateDto dto)
        {
            if (id <= 0)
                throw new ValidationException("Invalid ticket ID");

            if (dto == null)
                throw new ValidationException("Ticket data is required");

            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
                throw new NotFoundException($"Ticket with ID {id} not found");

            ticket.Price = dto.Price;
            ticket.SeatNumber = dto.SeatNumber;

            await _ticketRepository.UpdateAsync(ticket);
            return MapToDto(ticket);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Invalid ticket ID");

            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
                throw new NotFoundException($"Ticket with ID {id} not found");

            await _ticketRepository.DeleteAsync(id);
        }

        // ====== Helpers ======
        private static TicketReadDto MapToDto(Ticket t) => new()
        {
            Id = t.Id,
            EventId = t.EventId,
            RegistrationId = t.RegistrationId,
            UserId = t.UserId,
            Price = t.Price,
            SeatNumber = t.SeatNumber,
            CreatedAt = t.CreatedAt,
            PdfPath = t.PdfPath
        };

        private byte[] GenerateQrCode(string text)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }

        private string GeneratePdf(Ticket ticket, byte[] qrBytes)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tickets");
            Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, $"ticket_{ticket.Id}.pdf");

            using var writer = new PdfWriter(filePath, new WriterProperties().SetCompressionLevel(0));
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            document.Add(new Paragraph($"Event: {ticket.Event?.Title ?? "Unknown"}"));
            document.Add(new Paragraph($"Full Name: {ticket.Attendee?.FullName ?? "Unknown"}"));
            document.Add(new Paragraph($"Ticket ID: {ticket.Id}"));
            document.Add(new Paragraph($"Seat Number: {ticket.SeatNumber}"));
            document.Add(new Paragraph($"Price: {ticket.Price} JOD"));

            var qrImage = new Image(ImageDataFactory.Create(qrBytes));
            qrImage.ScaleAbsolute(100, 100);
            document.Add(qrImage);

            document.Close();
            return Path.Combine("tickets", $"ticket_{ticket.Id}.pdf");
        }
    }
}
    
