using Labb1ASP.NETDatabas.DTOs.AdministratorDTOs;
using Labb1ASP.NETDatabas.DTOs.BookingDTOs;
using Labb1ASP.NETDatabas.DTOs.CustomerDTOs;
using Labb1ASP.NETDatabas.DTOs.TableDTOs;
using Labb1ASP.NETDatabas.Models;

namespace Labb1ASP.NETDatabas.Extensions
{
    public static class MappingExtensions
    {
        // Table mappings
        public static TableResponseDto ToResponseDto(this Table table)
        {
            return new TableResponseDto
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                CurrentBookingsCount = table.Bookings?.Count(b => b.BookingDateTime.Date >= DateTime.Today) ?? 0
            };
        }

        public static Table ToEntity(this CreateTableDto dto)
        {
            return new Table
            {
                TableNumber = dto.TableNumber,
                Capacity = dto.Capacity,
                IsActive = true
            };
        }

        // Customer mappings
        public static CustomerResponseDto ToResponseDto(this Customer customer)
        {
            return new CustomerResponseDto
            {
                Id = customer.Id,
                Name = customer.Name,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                CreatedAt = customer.CreatedAt,
                RecentBookings = customer.Bookings?.OrderByDescending(b => b.BookingDateTime)
                    .Take(5)
                    .Select(b => new BookingSummaryDto
                    {
                        Id = b.Id,
                        BookingDateTime = b.BookingDateTime,
                        NumberOfGuests = b.NumberOfGuests,
                        TableNumber = b.Table?.TableNumber ?? 0
                    }).ToList() ?? new List<BookingSummaryDto>()
            };
        }

        public static Customer ToEntity(this CreateCustomerDto dto)
        {
            return new Customer
            {
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };
        }

        // Booking mappings
        public static BookingResponseDto ToResponseDto(this Booking booking)
        {
            return new BookingResponseDto
            {
                Id = booking.Id,
                BookingDateTime = booking.BookingDateTime,
                NumberOfGuests = booking.NumberOfGuests,
                SpecialRequests = booking.SpecialRequests,
                CreatedAt = booking.CreatedAt,
                Customer = new CustomerSummaryDto
                {
                    Id = booking.Customer?.Id ?? booking.CustomerId,
                    Name = booking.Customer?.Name ?? "Unknown",
                    PhoneNumber = booking.Customer?.PhoneNumber ?? "Unknown"
                },
                Table = new TableSummaryDto
                {
                    Id = booking.Table?.Id ?? booking.TableId,
                    TableNumber = booking.Table?.TableNumber ?? 0,
                    Capacity = booking.Table?.Capacity ?? 0
                }
            };
        }

        // Administrator mappings
        public static AdministratorResponseDto ToResponseDto(this Administrator admin)
        {
            return new AdministratorResponseDto
            {
                Id = admin.Id,
                Username = admin.Username,
                Name = admin.Name,
                Email = admin.Email,
                CreatedAt = admin.CreatedAt,
                LastLogin = admin.LastLogin
                // INGEN PasswordHash eller RefreshToken för säkerhet!
            };
        }

        public static Administrator ToEntity(this CreateAdministratorDto dto, string passwordHash)
        {
            return new Administrator
            {
                Username = dto.Username,
                PasswordHash = passwordHash,
                Name = dto.Name,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };
        }

        // MenuItem mappings
        public static MenuItemResponseDto ToResponseDto(this MenuItem menuItem)
        {
            return new MenuItemResponseDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                Category = menuItem.Category,
                ImageUrl = menuItem.ImageUrl,
                IsPopular = menuItem.IsPopular,
                IsAvailable = menuItem.IsAvailable,
                CreatedAt = menuItem.CreatedAt
            };
        }

        public static MenuItem ToEntity(this CreateMenuItemDto dto)
        {
            return new MenuItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category,
                ImageUrl = dto.ImageUrl,
                IsPopular = dto.IsPopular,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        // Update methods
        public static void UpdateFromDto(this Table table, UpdateTableDto dto)
        {
            if (dto.TableNumber.HasValue) table.TableNumber = dto.TableNumber.Value;
            if (dto.Capacity.HasValue) table.Capacity = dto.Capacity.Value;
            if (dto.IsActive.HasValue) table.IsActive = dto.IsActive.Value;
        }

        public static void UpdateFromDto(this Customer customer, UpdateCustomerDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Name)) customer.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) customer.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrEmpty(dto.Email)) customer.Email = dto.Email;
        }

        public static void UpdateFromDto(this Administrator admin, UpdateAdministratorDto dto, string? newPasswordHash = null)
        {
            if (!string.IsNullOrEmpty(dto.Username)) admin.Username = dto.Username;
            if (!string.IsNullOrEmpty(dto.Name)) admin.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Email)) admin.Email = dto.Email;
            if (!string.IsNullOrEmpty(newPasswordHash)) admin.PasswordHash = newPasswordHash;
        }

        public static void UpdateFromDto(this MenuItem menuItem, UpdateMenuItemDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Name)) menuItem.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description)) menuItem.Description = dto.Description;
            if (dto.Price.HasValue) menuItem.Price = dto.Price.Value;
            if (!string.IsNullOrEmpty(dto.Category)) menuItem.Category = dto.Category;
            if (dto.ImageUrl != null) menuItem.ImageUrl = dto.ImageUrl;
            if (dto.IsPopular.HasValue) menuItem.IsPopular = dto.IsPopular.Value;
            if (dto.IsAvailable.HasValue) menuItem.IsAvailable = dto.IsAvailable.Value;
        }

        public static void UpdateFromDto(this Booking booking, UpdateBookingDto dto)
        {
            if (dto.BookingDateTime.HasValue) booking.BookingDateTime = dto.BookingDateTime.Value;
            if (dto.NumberOfGuests.HasValue) booking.NumberOfGuests = dto.NumberOfGuests.Value;
            if (dto.SpecialRequests != null) booking.SpecialRequests = dto.SpecialRequests;
        }
    }
}
