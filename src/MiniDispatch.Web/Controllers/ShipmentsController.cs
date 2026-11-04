using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniDispatch.Application.Shipments.Queries.GetShipments;
using MiniDispatch.Domain.Enums;

namespace MiniDispatch.Web.Controllers
{
    [Route("[controller]")]
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
    }
}