using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WSVenta.Models;
using WSVenta.Models.Request;
using WSVenta.Models.Response;

namespace WSVenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VentaController : ControllerBase
    {
        [HttpPost]
        public IActionResult Add(VentaRequest model)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                using(VentaApiContext db = new VentaApiContext())
                {
                    using(var transaction = db.Database.BeginTransaction()) // para hacer transacion a la data base
                    {
                        try
                        {
                            var venta = new Venta();
                            venta.Total = model.Conceptos.Sum(d => d.Cantidad * d.PrecioUnitario);
                            venta.Fecha = DateTime.Now;
                            venta.IdCliente = model.IdCliente;
                            db.Add(venta);
                            db.SaveChanges();

                            foreach (var modelconcepto in model.Conceptos)
                            {
                                var concepto = new Models.Concepto();

                                concepto.IdVenta = venta.Id;
                                concepto.Cantidad = modelconcepto.Cantidad;
                                concepto.PrecioUnitario = modelconcepto.PrecioUnitario;
                                concepto.Importe = modelconcepto.Importe;
                                concepto.IdProducto = modelconcepto.IdProducto;

                                db.Conceptos.Add(concepto);
                                db.SaveChanges();
                            }

                            transaction.Commit(); // para que no se bloquee la database
                            respuesta.Exito = 1;

                        }
                        catch(Exception)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                respuesta.Mensaje = ex.Message;
            }

            return Ok(respuesta);
        }
    }
}
