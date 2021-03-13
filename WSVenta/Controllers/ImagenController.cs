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
    public class ImagenController : ControllerBase
    {
        [HttpPost]
        public ActionResult Add(Imagene Model)
        {
            try
            {
                using (VentaApiContext db = new VentaApiContext())
                {
                    Imagene img = new Imagene();
                    img.Nombre = Model.Nombre;            // Hay que verificar si guarda la imagen desde angular en la db
                    img.Imagen = Model.Imagen;

                    db.Imagenes.Add(img);
                    db.SaveChanges();
                }
            }
            catch (Exception) 
            {

            }
            return Ok();
        }
    }
}
