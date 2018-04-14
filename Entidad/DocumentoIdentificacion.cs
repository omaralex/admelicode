﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class DocumentoIdentificacion
    {
        public int idDocumento { get; set; }
        public string nombre { get; set; }
        public int numeroDigitos { get; set; }
        public string tipoDocumento { get; set; }
        public int estado { get; set; }

        private string estadoString;
        public string EstadoString
        {
            get
            {
                if (estado == 1) { return "Activo"; }
                else { return "Anulado"; }
            }
            set
            {
                estadoString = value;
            }
        }
    }
}
