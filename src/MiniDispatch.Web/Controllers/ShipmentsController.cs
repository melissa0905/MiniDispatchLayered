using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniDispatch.Application.Shipments.Commands.CreateShipment;
using MiniDispatch.Application.Shipments.Queries.GetShipments;
using MiniDispatch.Domain.Enums;

namespace MiniDispatch.Web.Controllers
{
    public class ShipmentsController(ISender sender) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(
        ShipmentStatus? status,
        CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (status.HasValue && !Enum.IsDefined(status.Value))
            {
                return BadRequest("Geçersiz sevkiyat durumu.");
            }

            var shipments = await sender.Send(
                new GetShipmentsQuery(status),
                cancellationToken);

            return View(shipments);
        }
    
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateShipmentCommand());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    CreateShipmentCommand command,
    CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            try
            {
                await sender.Send(command, cancellationToken);
            }
            catch (ValidationException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);

                return View(command);
            }

            TempData["SuccessMessage"] = "Sevkiyat başarıyla oluşturuldu.";

            return RedirectToAction(nameof(Index));
        }
    }
}
