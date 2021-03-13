using System;
using System.Collections.Generic;

#nullable disable

namespace WSVenta.Models
{
    public partial class Imagene
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public byte[] Imagen { get; set; }
    }
}
