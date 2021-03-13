using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WSVenta.Models;
using WSVenta.Models.Request;
using WSVenta.Models.Response;
using WSVenta.Services;
using WSVenta.Tools;

namespace WSVenta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Autentificar([FromBody] AuthRequest model)
        {
            Respuesta respuesta = new Respuesta();

            var userresponse = _userService.Auth(model);

            if (userresponse == null)
            {
                respuesta.Exito = 0;
                respuesta.Mensaje = "Usuario o contraseña incorrecta";
                return BadRequest(respuesta);

            }

            respuesta.Exito = 1;
            respuesta.Data = userresponse;

            return Ok(respuesta);
        }

        [HttpGet]
        public ActionResult Get()
        {
            Respuesta respuesta = new Respuesta();
            respuesta.Exito = 0;

            try
            {
                using (VentaApiContext db = new VentaApiContext())
                {
                    var lst = db.Usuarios.OrderByDescending(d => d.Id).ToList();
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
        public ActionResult Add(UserRequest Model)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.Exito = 0;

            try
            {
                using (VentaApiContext db = new VentaApiContext())
                {
                    Encrypt.GetSHA256(Model.Password);
                    Usuario us = new Usuario();
                    us.Nombre = Model.Nombre;
                    us.Apellido = Model.Apellido;
                    us.Email = Model.Email;
                    us.Password = Encrypt.GetSHA256(Model.Password);
                    db.Usuarios.Add(us);
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
        public ActionResult Edit(UserRequest Model)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.Exito = 0;

            try
            {
                using (VentaApiContext db = new VentaApiContext())
                {
                    Usuario us = db.Usuarios.Find(Model.Id);
                    us.Nombre = Model.Nombre;
                    us.Apellido = Model.Apellido;
                    us.Email = Model.Email;
                    us.Password = Encrypt.GetSHA256(Model.Password);
                    db.Entry(us).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
