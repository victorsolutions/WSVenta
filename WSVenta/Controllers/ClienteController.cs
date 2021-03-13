using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WSVenta.Models;
using WSVenta.Models.Response;
using WSVenta.Models.Request;

namespace WSVenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get ()
        {
            Respuesta respuesta = new Respuesta();
            respuesta.Exito = 0;

            try
            {
                using (VentaApiContext db = new VentaApiContext())
                {
                    var lst = db.Clientes.OrderByDescending(d => d.Id).ToList();
                    respuesta.Data = lst;
                }
                respuesta.Exito = 1;
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = ex.Message;
            }
            return Ok(respuesta);
        }

        [HttpPost]
        public ActionResult Add(ClienteRequest Model)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.Exito = 0;

            try
            {
                using (VentaApiContext db = new VentaApiContext())
                {
                    Cliente Clt = new Cliente();
                    Clt.Nombre = Model.Nombre;
                    Clt.Apellido = Model.Apellido;
                    db.Clientes.Add(Clt);
                    db.SaveChanges();
                }
                respuesta.Exito = 1;
            }
            catch (Exception ex) 
            {
                respuesta.Mensaje = ex.Message;
            }
            return Ok(respuesta);
        }

        [HttpPut]
        public ActionResult Edit(ClienteRequest Model)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.Exito = 0;

            try
            {
                using (VentaApiContext db = new VentaApiContext())
                {
                    Cliente Clt = db.Clientes.Find(Model.Id);
                    Clt.Nombre = Model.Nombre;
                    Clt.Apellido = Model.Apellido;
                    db.Entry(Clt).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    db.SaveChanges();                 
                }
                respuesta.Exito = 1;
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = ex.Message;
            }
            return Ok(respuesta);
        }

        [HttpDelete("{Id}")]
        public ActionResult Delete(int Id)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.Exito = 0;

            try
            {
                using (VentaApiContext db = new VentaApiContext())
                {
                    Cliente Clt = db.Clientes.Find(Id);
                    db.Remove(Clt);
                    db.SaveChanges();
                }
                respuesta.Exito = 1;
            }
            catch (Exception ex)
            {
                respuesta.Mensaje = ex.Message;
            }
            return Ok(respuesta);
        }
    }
}
