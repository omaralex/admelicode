﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class Response
    {
        public string msj { get; set; }
        public int id { get; set; }
    }

    public class ResponseD
    {
        public string msj { get; set; }
        public object id { get; set; }
    }
    public class ResponseNota
    {
        public string idProducto { get; set; }
        public string idCombinacionAlternativa { get; set; }
        public string existeProducto { get; set; }
        public int cumple { get; set; }
    }
    public class ResponseNotaGuardar
    {
        public string msj { get; set; }
        public int id { get; set; }
        public string serie { get; set; }
        public string correlativo { get; set; }
        public string fecha { get; set; }
    }


   

}
