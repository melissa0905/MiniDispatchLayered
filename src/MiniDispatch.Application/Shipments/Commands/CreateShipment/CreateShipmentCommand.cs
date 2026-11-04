using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace MiniDispatch.Application.Shipments.Commands.CreateShipment
{
    public class CreateShipmentCommand : IRequest<Guid>
    {
        [Required(ErrorMessage = "Referans numarası zorunludur.")]
        [StringLength(30, ErrorMessage = "En fazla 30 karakter girilebilir.")]
        public string ReferenceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Çıkış noktası zorunludur.")]
        [StringLength(100, ErrorMessage = "En fazla 100 karakter girilebilir.")]
        public string Origin { get; set; } = string.Empty;

        [Required(ErrorMessage = "Varış noktası zorunludur.")]
        [StringLength(100, ErrorMessage = "En fazla 100 karakter girilebilir.")]
        public string Destination { get; set; } = string.Empty;

        [Required(ErrorMessage = "Araç plakası zorunludur.")]
        [StringLength(20, ErrorMessage = "En fazla 20 karakter girilebilir.")]
        public string VehiclePlate { get; set; } = string.Empty;
    }
}