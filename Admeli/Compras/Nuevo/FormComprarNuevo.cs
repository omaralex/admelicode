﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Admeli.Compras.Buscar;
using Entidad;
using Entidad.Configuracion;
using Modelo;

namespace Admeli.Compras.Nuevo
{
    public partial class FormComprarNuevo : Form
    {

        // compra 
        PagoC pagoC;
        CompraC compraC;
        List< DetalleC> detalleC;
        PagocompraC pagocompraC;
        List< DatoNotaEntradaC> datoNotaEntradaC;
        NotaentradaC notaentrada;
        compraTotal compraTotal;

        private MonedaModel monedaModel = new MonedaModel();
        private TipoDocumentoModel tipoDocumentoModel = new TipoDocumentoModel();
        private ProductoModel productoModel = new ProductoModel();
        private AlternativaModel alternativaModel = new AlternativaModel();
        private PresentacionModel presentacionModel = new PresentacionModel();
        private FechaModel fechaModel = new FechaModel();
        private CompraModel compraModel = new CompraModel();
        private MedioPagoModel medioPagoModel = new MedioPagoModel();
        private AlmacenModel almacenModel = new AlmacenModel();
        /// Sus datos se cargan al abrir el formulario
        private List<Moneda> monedas { get; set; }
        private List<TipoDocumento> tipoDocumentos { get; set; }
        private FechaSistema fechaSistema { get; set; }
        private List<Producto> productos { get; set; }
        private List<MedioPago> medioPagos { get; set; }

        /// Llenan los datos en las interacciones en el formulario 
        private List<Presentacion> presentaciones { get; set; }
        private Producto currentProducto { get; set; }
        private Proveedor currentProveedor { get; set; }
        private Sucursal sucursal { get; set; }
        private Personal personal { get; set; }

        /// Se preparan para realizar la compra de productos
        private List<DetalleCompra> detalleCompras { get; set; }
        private Compra currentCompra { get; set; } // notaEntrada,pago,pagoCompra
        private NotaEntrada currentNotaEntrada { get; set; }
        private Pago currentPago { get; set; }
        private PagoCompra currentPagoCompra { get; set; }


        private AlmacenComra currentAlmacenCompra { get; set; }
        private List<AlmacenComra> Almacen { get; set; }
        private bool nuevo { get; set; }

        #region ============================ Constructor ============================
        public FormComprarNuevo(Sucursal sucursal, Personal personal)
        {
            InitializeComponent();
            this.nuevo = true;
            cargarFechaSistema();
            this.personal = personal;
            this.sucursal = sucursal;
            pagoC=new PagoC();
             compraC=new CompraC();
             detalleC=new List<DetalleC>();
             pagocompraC = new PagocompraC() ;
            datoNotaEntradaC = new List<DatoNotaEntradaC>();
             notaentrada=new NotaentradaC();
             compraTotal=new compraTotal();
        }

        public FormComprarNuevo(Compra currentCompra, Sucursal sucursal, Personal personal)
        {
            InitializeComponent();
            this.currentCompra = currentCompra;

            this.nuevo = false;
        }
        #endregion

        #region ============================== CRUDS ==============================
        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            executeBuscarProveedor();
        } 
        #endregion

        #region ================================ Root Load ================================
        private void FormComprarNuevo_Load(object sender, EventArgs e)
        {
            this.reLoad();
        }

        private void reLoad()
        {
            cargarMonedas();
            cargarTipoDocumento();
            cargarFechaSistema();
            cargarProductos();
            cargarMedioPago();
            cargarAlmacen();
        }
        #endregion

        #region ============================== Load ==============================
        private async void cargarMonedas()
        {
            try
            {
                monedas = await monedaModel.monedas();
                cbxMoneda.DataSource = monedas;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Listar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void cargarTipoDocumento()
        {
            try
            {
                tipoDocumentos = await tipoDocumentoModel.tipoDocumentoVentas();
                cbxTipoDocumento.DataSource = tipoDocumentos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Listar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void cargarFechaSistema()
        {
            try
            {
                if (!nuevo) return;
                fechaSistema = await fechaModel.fechaSistema();
                dtpEmision.Value = fechaSistema.fecha;
                dtpPago.Value = fechaSistema.fecha;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Listar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void cargarProductos()
        {
            try
            {
                productos = await productoModel.productos();
                productoBindingSource.DataSource = productos;
                cbxCodigoProducto.SelectedIndex = -1;
                cbxDescripcion.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Listar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void cargarMedioPago()
        {
            try
            {
                medioPagos = await medioPagoModel.medioPagos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Listar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion
        private async void cargarAlmacen()
        {

           Almacen = await almacenModel.almacenesCompra(sucursal.idSucursal,personal.idPersonal);
           currentAlmacenCompra = Almacen[0];
        }


        #region ============================  Agregar Producto Al Carro ============================
        private void cbxCodigoProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarProductoDetalle();
        }

        private void cbxDescripcion_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarProductoDetalle();
        }

        private void textCantidad_OnValueChanged(object sender, EventArgs e)
        {
            calcularTotal();
        }

        private void textDescuento_OnValueChanged(object sender, EventArgs e)
        {
            calcularTotal();
        }

        private void cbxUnidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            calcularPrecioUnitario();
            calcularTotal();
        }

        private void btnAddCard_Click(object sender, EventArgs e)
        {
            if(detalleCompras == null) detalleCompras = new List<DetalleCompra>();
            DetalleCompra detalleCompra = new DetalleCompra();

            // Creando la lista
            detalleCompra.cantidad = double.Parse(textCantidad.Text.Trim(), CultureInfo.GetCultureInfo("en-US"));

            string ff = cbxPresentacion.SelectedValue.ToString();
            /// Busqueda presentacion

            Presentacion findPresentacion = presentaciones.Find(x => x.idPresentacion == Convert.ToInt32(cbxPresentacion.SelectedValue));
            detalleCompra.cantidadUnitaria = double.Parse(findPresentacion.cantidadUnitaria, CultureInfo.GetCultureInfo("en-US"));

            detalleCompra.codigoProducto = cbxCodigoProducto.Text.Trim();
            detalleCompra.descripcion = cbxDescripcion.Text.Trim();
            detalleCompra.descuento = Convert.ToDouble(textDescuento.Text.Trim());
            detalleCompra.estado = 1;
            detalleCompra.idCombinacionAlternativa = Convert.ToInt32(cbxCombinacion.SelectedValue);

            detalleCompra.idCompra = 0;
            detalleCompra.idDetalleCompra = 0;
            detalleCompra.idPresentacion = Convert.ToInt32(cbxPresentacion.SelectedValue);

            detalleCompra.idProducto = Convert.ToInt32(cbxCodigoProducto.SelectedValue);
            detalleCompra.idSucursal = ConfigModel.sucursal.idSucursal;
            detalleCompra.nombreCombinacion = cbxCombinacion.Text;
            detalleCompra.nombreMarca = currentProducto.nombreMarca;
            detalleCompra.nombrePresentacion = cbxPresentacion.Text;
            detalleCompra.nro = 1;
            detalleCompra.precioUnitario = double.Parse(textPrecioUnidario.Text.Trim(), CultureInfo.GetCultureInfo("en-US"));
            detalleCompra.total = double.Parse(textTotal.Text.Trim(), CultureInfo.GetCultureInfo("en-US"));

            // agrgando un nuevo item a la lista
            detalleCompras.Add(detalleCompra);

            // Refrescando la tabla
            detalleCompraBindingSource.DataSource = null;
            detalleCompraBindingSource.DataSource = detalleCompras;
            dataGridView.Refresh();

            // Calculo de totales y subtotales
            calculoSubtotal();
        }

        private void cargarProductoDetalle()
        {
            if (cbxCodigoProducto.SelectedIndex == -1 || cbxDescripcion.SelectedIndex == -1) return;
            try
            {
                /// Buscando el producto seleccionado
                int idProducto = Convert.ToInt32(cbxCodigoProducto.SelectedValue) | Convert.ToInt32(cbxDescripcion.SelectedValue);
                currentProducto = productos.Find(x => x.idProducto == idProducto);

                // Llenar los campos del producto escogido
                textCantidad.Text = "1";
                textDescuento.Text = "0";

                /// Cargando presentaciones
                cargarPresentaciones();

                /// Cargando alternativas del producto
                cargarAlternativas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Listar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void cargarPresentaciones()
        {
            if (cbxCodigoProducto.SelectedIndex == -1 || cbxDescripcion.SelectedIndex == -1) return; /// validacion
                                                                                                     /// Cargar las precentaciones
            string f = cbxCodigoProducto.SelectedValue.ToString();
            presentaciones = await presentacionModel.presentacionVentas(Convert.ToInt32(cbxCodigoProducto.SelectedValue));
            presentacionBindingSource.DataSource = presentaciones;
            //cbxPresentacion.SelectedIndex = -1;

            /// calculos
            calcularPrecioUnitario();
            calcularTotal();
        }

        private async void cargarAlternativas()
        {
            if (cbxCodigoProducto.SelectedIndex == -1 || cbxDescripcion.SelectedIndex == -1) return; /// validacion
            /// cargando las alternativas del producto
            List<AlternativaCombinacion> alternativaCombinacion = await alternativaModel.cAlternativa31(Convert.ToInt32(cbxCodigoProducto.SelectedValue));
            alternativaCombinacionBindingSource.DataSource = alternativaCombinacion;

            /// calculos
            calcularPrecioUnitario();
            calcularTotal();
        }

        private void calculoSubtotal()
        {
            double subtotal = 0;
            foreach (DetalleCompra item in detalleCompras)
            {
                subtotal += item.total;
            }

            textSubTotal.Text = subtotal.ToString();
            double impuesto = double.Parse(textImpuesto.Text, CultureInfo.GetCultureInfo("en-US"));
            textTotalNeto.Text = (subtotal + impuesto).ToString(); 
        }

        /// <summary>
        /// Calcular Total
        /// </summary>
        private void calcularTotal()
        {
            try
            {
                if (textCantidad.Text.Trim() == "") textTotal.Text = "0";
                if (textPrecioUnidario.Text.Trim() == "" || textCantidad.Text.Trim() == "" || textDescuento.Text.Trim() == "") return; /// Validación

                double precioUnidario = double.Parse(textPrecioUnidario.Text, CultureInfo.GetCultureInfo("en-US"));
                double cantidad = double.Parse(textCantidad.Text, CultureInfo.GetCultureInfo("en-US"));
                double descuento = double.Parse(textDescuento.Text, CultureInfo.GetCultureInfo("en-US"));
                double total = (precioUnidario * cantidad) - descuento;

                textTotal.Text = string.Format(CultureInfo.GetCultureInfo("en-US"), "{0}", total);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Calcular total", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Calcular Precio Unitario
        /// </summary>
        private void calcularPrecioUnitario() {
            if (cbxCodigoProducto.SelectedIndex == -1 || cbxDescripcion.SelectedIndex == -1) return; /// Validación
            try
            {
                if (cbxPresentacion.SelectedIndex == -1)
                {
                    textPrecioUnidario.Text = currentProducto.precioCompra;
                }
                else
                {
                    // Buscar presentacion elegida
                    int idPresentacion = Convert.ToInt32(cbxPresentacion.SelectedValue);
                    Presentacion findPresentacion = presentaciones.Find(x => x.idPresentacion == idPresentacion);

                    // Realizando el calculo
                    double precioCompra = double.Parse(currentProducto.precioCompra, CultureInfo.GetCultureInfo("en-US"));
                    double cantidadUnitario = double.Parse(findPresentacion.cantidadUnitaria, CultureInfo.GetCultureInfo("en-US"));
                    double precioUnidatio = precioCompra * cantidadUnitario;

                    // Imprimiendo valor
                    textPrecioUnidario.Text = String.Format(CultureInfo.GetCultureInfo("en-US"), "{0}", precioUnidatio);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Calcular total", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        private void executeBuscarProveedor()
        {
            BuscarProveedor buscarProveedor = new BuscarProveedor();
            buscarProveedor.ShowDialog();
            currentProveedor = buscarProveedor.currentProveedor;
            mostrarProveedor();
        }

        private void mostrarProveedor()
        {
            if (currentProveedor != null)
            {
                textNombreEmpresa.Text = currentProveedor.razonSocial;
                textDireccion.Text = currentProveedor.direccion;
            }
        }

        private void textNombreEmpresa_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            executeBuscarProveedor();
        }


        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            BuscarProducto buscarProducto = new BuscarProducto();
            buscarProducto.ShowDialog();

        }

        private void cargarProductoSeleccionado()
        {
            if (currentProducto == null) return;
        }

        #region =============================== Realizar La Compra ===============================
        private async void   btnRealizarCompra_Click(object sender, EventArgs e)
        {

            //pago
            pagoC.estado = 8;// ver que significado
            pagoC.estadoPago = 1;//ver que significado

            // Moneda aux = monedaBindingSource.;
            int i = cbxMoneda.SelectedIndex;
            //  Moneda aux = monedaBindingSource.List[i] as Moneda;
            pagoC.idMoneda = monedas[i].idMoneda;

            pagoC.idPago = 0;
            pagoC.motivo = "COMPRA";
            pagoC.saldo = Convert.ToDouble(textTotal.Text);
            pagoC.valorPagado = 0;
            pagoC.valorTotal = Convert.ToDouble(textTotal.Text);
            // compra
            string date1 = String.Format("{0:u}", dtpEmision.Value);
            date1 = date1.Substring(0, date1.Length - 1);
            string date= String.Format("{0:u}", dtpPago.Value);
            date = date.Substring(0, date.Length - 1);

            compraC.idCompra = 0;
            compraC.numeroDocumento = "0";
            compraC.rucDni = currentProveedor.ruc;
            compraC.direccion = currentProveedor.direccion;
            compraC.formaPago = "EFECTIVO";
            compraC.fechaPago = date;
            compraC.fechaFacturacion = date1;
            compraC.descuento = textDescuento.Text;
            compraC.tipoCompra = "Con productos";
            compraC.subTotal = Convert.ToDouble(textSubTotal.Text);
            compraC.total = Convert.ToDouble(textTotal.Text);
            compraC.observacion = textObservacion.Text;
            compraC.estado = 1;
            compraC.idProveedor = currentProveedor.idProveedor;
            compraC.nombreProveedor = currentProveedor.razonSocial;
            compraC.idPago = 0;
            compraC.idPersonal = PersonalModel.personal.idPersonal;
            compraC.tipoCambio = 1;
            int j = cbxTipoDocumento.SelectedIndex;
            TipoDocumento aux = cbxTipoDocumento.Items[j] as TipoDocumento;
            compraC.idTipoDocumento = (aux.idTipoDocumento);
            compraC.idSucursal = ConfigModel.sucursal.idSucursal;
            compraC.nombreLabel = aux.nombreLabel;
            compraC.vendedor = PersonalModel.personal.nombres;
            compraC.nroOrdenCompra = "";
            compraC.moneda = monedas[i].moneda;
            compraC.idCompra= monedas[i].idMoneda;
            //detalle


            foreach (DetalleCompra detalle in detalleCompras)
            {
                DetalleC aux1 = new DetalleC();

                aux1.cantidad = Convert.ToInt32(detalle.cantidad);
                aux1.cantidadUnitaria = Convert.ToInt32(detalle.cantidadUnitaria);
                aux1.codigoProducto = detalle.codigoProducto;
                aux1.descripcion = detalle.descripcion;
                aux1.descuento = detalle.descuento;
                aux1.estado = detalle.estado;
                aux1.idCombinacionAlternativa = detalle.idCombinacionAlternativa;
                aux1.idCompra = detalle.idCompra;
                aux1.idDetalleCompra = detalle.idDetalleCompra;
                aux1.idPresentacion = detalle.idPresentacion;
                aux1.idProducto = detalle.idProducto;
                aux1.idSucursal = detalle.idSucursal;
                aux1.nombreCombinacion = detalle.nombreCombinacion;
                aux1.nombreMarca = detalle.nombreMarca;
                aux1.nombrePresentacion = detalle.nombrePresentacion;
                aux1.nro = detalle.nro;
                aux1.precioUnitario = detalle.precioUnitario;
                aux1.total = detalle.total;
                detalleC.Add(aux1);

                DatoNotaEntradaC aux2 = new DatoNotaEntradaC();
                aux2.idProducto = detalle.idProducto;
                aux2.cantidad = detalle.cantidad;
                aux2.idCombinacionAlternativa = detalle.idCombinacionAlternativa;
                aux2.idAlmacen = currentAlmacenCompra.idAlmacen;
                aux2.descripcion = detalle.descripcion;

                datoNotaEntradaC.Add(aux2);

                

            }




            pagocompraC.idCaja = FormPrincipal.asignacion.idCaja;
            pagocompraC.idPago = 0;
            pagocompraC.moneda = monedas[i].moneda;
            pagocompraC.idMoneda = monedas[i].idMoneda;
            pagocompraC.idMedioPago = medioPagos[0].idMedioPago;
            pagocompraC.idCajaSesion = ConfigModel.cajaSesion.idCajaSesion;
            pagocompraC.pagarCompra = 1;

            notaentrada.datoNotaEntrada = datoNotaEntradaC;
            notaentrada.generarNotaEntrada = chbxNotaEntrada.Checked == true ? 1 : 0;
            notaentrada.idCompra = 0;
            notaentrada.idTipoDocumento = aux.idTipoDocumento;
            notaentrada.idPersonal = PersonalModel.personal.idPersonal;


            compraTotal = new compraTotal();
            compraTotal.detalle = detalleC;
            compraTotal.compra = compraC;
            compraTotal.notaentrada = notaentrada;
            compraTotal.pago = pagoC;
            compraTotal.pagocompra = pagocompraC;




             await compraModel.ralizarCompra(compraTotal);


        }

        
        private void crearObjetoCompra()
        {
            Compra compra = new Compra();
            compra.descuento = textDescuentoCompra.Text;
            compra.direccion = textDireccion.Text;
            compra.estado = 1;
            // compra.fechaFacturacion = dtpEmision.Value.ToString("yyyy-MMM-dd  hh:mm:ss");
            //compra.fechaPago = "";
            compra.formaPago = "";

        }


        #endregion
    }
}
