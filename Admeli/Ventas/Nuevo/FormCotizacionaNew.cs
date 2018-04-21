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
using Admeli.Componentes;
using Admeli.Compras.buscar;
using Admeli.Compras.Buscar;
using Admeli.Productos.Nuevo;
using Admeli.Ventas.Buscar;
using Entidad;
using Entidad.Configuracion;
using Entidad.Location;
using Modelo;
using Newtonsoft.Json;

namespace Admeli.Ventas.Nuevo
{
    public partial class FormCotizacionaNew : Form
    {


        //variables para realizar  un orden de compra ordenCompra

        CotizacionG cotizacionG { get; set; }
        TotalCotizacion totalCotizacion { get; set; }


       
       
       


        //webservice utilizados
      
        private TipoDocumentoModel tipoDocumentoModel = new TipoDocumentoModel();
        private DocCorrelativoModel docCorrelativoModel = new DocCorrelativoModel();       
        private AlternativaModel alternativaModel = new AlternativaModel();      
        private FechaModel fechaModel = new FechaModel();    
        private MedioPagoModel medioPagoModel = new MedioPagoModel();       
        private LocationModel locationModel = new LocationModel();       
        private MonedaModel monedaModel = new MonedaModel();
        private DocumentoIdentificacionModel documentoIdentificacionModel = new DocumentoIdentificacionModel();
        private ClienteModel clienteModel = new ClienteModel();
        private CotizacionModel cotizacionModel = new CotizacionModel();
        private ImpuestoModel impuestoModel = new ImpuestoModel();
        private ProductoModel productoModel = new ProductoModel();
        private PresentacionModel presentacionModel = new PresentacionModel();
        private DescuentoModel descuentoModel = new DescuentoModel();
        private StockModel stockModel = new StockModel();



        /// Sus datos se cargan al abrir el formulario
        private List<Moneda> monedas { get; set; }
        private List<TipoDocumento> tipoDocumentos { get; set; }
        private FechaSistema fechaSistema { get; set; }      
        private List<MedioPago> medioPagos { get; set; }          
        private List<DocumentoIdentificacion> documentoIdentificacion { get; set; }
        private List<Cliente> listClientes { get; set; }
        private List<Impuesto> listImpuesto { get; set; }
        private List<ImpuestoDocumento> listIDocumento { get; set; }
        private List<ProductoVenta> listProductos { get; set; }
        private DescuentoProductoReceive  descuentoProducto{ get; set; }
        private List<DetalleV> detalleVentas { get; set; }
        private List<Presentacion> listPresentacion { get; set; }
        private List<ImpuestoProducto> listImpuestosProducto { get; set; }
        public UbicacionGeografica CurrentUbicacionGeografica;      
        /// Llenan los datos en las interacciones en el formulario 
     
       
      
        private Presentacion currentPresentacion { get; set; }      
        private CorrelativoCotizacion correlativoCotizacion { get; set; }
        private ProductoVenta currentProducto { get; set; }
        private ImpuestoProducto impuestoProducto { get; set; }
        private Cliente CurrentCliente { get; set; }


        private Cotizacion  currentCotizacion { get; set; }










        public int nroNuevo = 0;
       
        private bool nuevo { get; set; }

        private int idPresentacionDatagriview = 0;    
        int nroDecimales = 2;
        string formato { get; set; }
        private int nroCaracteres = 0;
        private double subTotal = 0;
        private double Descuento = 0;
        private double impuesto = 0;
        private double total = 0;
        
        public FormCotizacionaNew()
        {
            InitializeComponent();


       
           
            this.nuevo = true;
            cargarFechaSistema();
          
                
            formato = "{0:n" + nroDecimales + "}";
            
           
            cargarResultadosIniciales();

        }

     
        #region============= metods de apoyo en formato de decimales

        private void cargarResultadosIniciales()
        {


            lbSubtotal.Text = "s/" + ". " + darformato(0);
            lbDescuentoVentas.Text = "s/" + ". " + darformato(0);
            lbImpuesto.Text = "s/" + ". " + darformato(0);
            lbTotalCompra.Text = "s/" + ". " + darformato(0);

        }


        private string darformato(object dato)
        {
            return string.Format(CultureInfo.GetCultureInfo("en-US"), this.formato, dato);
        }
        private string darformatoGuardar(object dato)
        {

            string var1 = string.Format(CultureInfo.GetCultureInfo("en-US"), this.formato, dato);
            var1= var1.Replace(",","");
            return var1;
        }

        private double toDouble(string texto)
        {
            return double.Parse(texto, CultureInfo.GetCultureInfo("en-US")); ;
        }
        private int toEntero(string texto)
        {
            return Int32.Parse(texto, CultureInfo.GetCultureInfo("en-US")); ;
        }


        #endregion============================

        public FormCotizacionaNew(Cotizacion currentCotizacion)
        {
            
            InitializeComponent();
            this.currentCotizacion = currentCotizacion; 
            this.nuevo = false;       
            formato = "{0:n" + nroDecimales + "}";
         
        }
        #region ================================ Root Load ================================
        private void FormCompraNew_Load(object sender, EventArgs e)
        {


            if (nuevo == true)
            {
                this.reLoad();
               
               
            }
            else
            {
                this.reLoad();

                cargarCotizacion();                    
            }

            AddButtonColumn();

            btnModificar.Enabled = false;
        }
        private void reLoad()
        {
                      
            cargarMonedas();        
            cargarFechaSistema();
            cargarProductos();                             
            cargartiposDocumentos();
            cargarClientes();
            cargarCorrelactivo();
            cargarImpuesto();
            cargarPresentacion();
            cargarObjetos();


        }
        #endregion
        #region ============================== Load ==============================


        private async void cargarCotizacion()
        {
            detalleVentas=  await cotizacionModel.detalleCotizacion(currentCotizacion.idCotizacion);
           
            txtObservaciones.Text = currentCotizacion.observacion;
           

            double impuesto = 0; 
            foreach(DetalleV V in detalleVentas)
            {
               
                double Im = toDouble(V.valor);
                impuesto += Im;


                double precioUnitario = toDouble(V.precioUnitario);
                double cantidad = toDouble(V.cantidad);
                double cantidad1 = toDouble(V.cantidadUnitaria);

                V.precioUnitario = darformato(precioUnitario);
                V.cantidad = darformato(cantidad);
                V.cantidadUnitaria= darformato(cantidad1);

                double total1 = toDouble(V.total);
                V.total = darformato(total1);             
                double total = precioUnitario * cantidad + Im;
                V.totalGeneral = darformato(total);
                V.precioVenta = darformato(total/ cantidad);

                double precioVenta = toDouble(V.precioVenta);
                double d=  1-toDouble(V.descuento) / 100;
                


                V.precioVentaReal = darformato(precioVenta/d);
                listImpuestosProducto = await impuestoModel.impuestoProducto(V.idProducto, ConfigModel.sucursal.idSucursal);
                // calculamos lo impuesto posibles del producto
                double porcentual = 0;
                double efectivo = 0;
                foreach (ImpuestoProducto I in listImpuestosProducto)
                {
                    if (I.porcentual)
                    {
                        porcentual += toDouble(I.valorImpuesto);
                    }
                    else
                    {
                        efectivo += toDouble(I.valorImpuesto);
                    }
                }
                V.Porcentual = darformato(porcentual);
                V.Efectivo = darformato(efectivo);


            }
            detalleVBindingSource.DataSource = detalleVentas;

           


            limpiarCamposProducto();

        }
        private void cargarObjetos() {

            cotizacionG = new CotizacionG();
            totalCotizacion = new TotalCotizacion();
        }
        private async void cargarPresentacion()
        {                                                                                                    /// Cargar las precentaciones
            try
            {
                listPresentacion = await presentacionModel.presentacionesTodas();
                presentacionBindingSource.DataSource = listPresentacion;
                cbxDescripcion.SelectedIndex = -1;
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
                listProductos = await productoModel.productos(ConfigModel.sucursal.idSucursal, PersonalModel.personal.idPersonal);
                productoVentaBindingSource.DataSource = listProductos;
                cbxCodigoProducto.SelectedIndex = -1;
                cbxDescripcion.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Listar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private async void cargarClientes()
        {
            try
            {

                listClientes = await clienteModel.ListarClientesActivos();
                clienteBindingSource.DataSource = listClientes;
                if (!nuevo)
                {
                    cbxCliente.SelectedValue = currentCotizacion.idCliente;

                }

            }
            catch ( Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "cargar Clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
                    
        }
        private async void cargartiposDocumentos()
        {

            try
            {

                documentoIdentificacion = await documentoIdentificacionModel.docIdentificacion();
                documentoIdentificacionBindingSource.DataSource = documentoIdentificacion;

                

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "cargar Doc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }       
        }
        private async void cargarImpuesto()
        {
            try
            {
                // variables necesarios para el calculo del impuesto de la venta
                listImpuesto = await impuestoModel.listarImpuesto();

                listIDocumento = await impuestoModel.impuestoTipoDoc(ConfigModel.sucursal.idSucursal, 4); // tipo documento 4 

              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "cargar Impuesto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        private async void cargarCorrelactivo()
        {
            if (nuevo)
            {


                List<CorrelativoCotizacion> list = await cotizacionModel.Correlativo(ConfigModel.sucursal.idSucursal);
                correlativoCotizacion = list[0];
                txtComprobante.Text = "COTIZACION";
                txtSerie.Text = correlativoCotizacion.serie;
                txtCorrelativo.Text = correlativoCotizacion.correlativoActual;

            }
            else
            {
                txtComprobante.Text = "COTIZACION";
                txtSerie.Text = currentCotizacion.serie;
                txtCorrelativo.Text = currentCotizacion.correlativo;

            }

          
        }
        private async void cargarMonedas()
        {
            try
            {
                monedas = await monedaModel.monedas();
                cbxTipoMoneda.DataSource = monedas;
                if (!nuevo)
                {

                    Moneda moneda = monedas.Find(X => X.idMoneda == (int)cbxTipoMoneda.SelectedValue);  
                    cbxTipoMoneda.SelectedValue = currentCotizacion.idMoneda;
                    txtObservaciones.Text = currentCotizacion.observacion;
                    this.Descuento = toDouble(currentCotizacion.descuento);

                    lbDescuentoVentas.Text = moneda.simbolo + ". " + darformato(Descuento);

                    this.total = toDouble(currentCotizacion.total);
                    lbTotalCompra.Text = moneda.simbolo + ". " + darformato(total);

                    this.subTotal = toDouble(currentCotizacion.subTotal);
                    lbSubtotal.Text = moneda.simbolo + ". " + darformato(subTotal);
                    double impuesto = total - subTotal;
                    lbImpuesto.Text = moneda.simbolo + ". " + darformato(impuesto);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "cargar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private async void cargarFechaSistema()
        {
            try
            {
                if (!nuevo)
                {
                    dtpEmision.Value = currentCotizacion.fechaEmision.date;
                    dtpFechaVecimiento.Value= currentCotizacion.fechaVencimiento.date;
               
                }
                else
                {
                    fechaSistema = await fechaModel.fechaSistema();
                    dtpEmision.Value = fechaSistema.fecha;
                    dtpFechaVecimiento.Value = fechaSistema.fecha;

                }

                //dtpFechaPago.Value = fechaSistema.fecha;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "cargar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
       










                      
        private void AddButtonColumn()
        {
            DataGridViewButtonColumn buttons = new DataGridViewButtonColumn();
            {
                buttons.HeaderText = "Acciones";
                buttons.Text = "X";
                buttons.UseColumnTextForButtonValue = true;
                //buttons.AutoSizeMode =
                //   DataGridViewAutoSizeColumnMode.AllCells;
                buttons.FlatStyle = FlatStyle.Popup;
                buttons.CellTemplate.Style.BackColor = Color.Red;
                buttons.CellTemplate.Style.ForeColor = Color.White;

                buttons.Name = "acciones";           
            }

            dgvDetalleOrdenCompra.Columns.Add(buttons);

        }
        
        

        
       

        // para el cbx de descripcion
       
       

        #endregion

        #region=========== METODOS DE APOYO EN EL CALCULO


        private  void calculoSubtotal()
        {

            if (cbxTipoMoneda.SelectedValue == null)
                return;
          
            double subTotalLocal = 0;
            double TotalLocal = 0;
            foreach (DetalleV  item in detalleVentas)
            {
                if(item.estado==1)
                subTotalLocal += toDouble( item.total);
                TotalLocal += toDouble(item.totalGeneral);
            }


            Moneda moneda = monedas.Find(X => X.idMoneda == (int)cbxTipoMoneda.SelectedValue);
            this.subTotal = subTotalLocal;

            lbSubtotal.Text = moneda.simbolo + ". " + darformato(subTotalLocal);
            // determinar impuesto de cada producto
            double impuestoTotal = TotalLocal- subTotalLocal;

            // arreglar esto esta mal la logica ya que el impuesto es procentual
           
            this.impuesto = impuestoTotal;
            lbImpuesto.Text = moneda.simbolo + ". " + darformato(impuestoTotal);

            // determinar impuesto de cada producto
            this.total = TotalLocal;
            lbTotalCompra.Text = moneda.simbolo + ". " + darformato(TotalLocal);

        }

        /// <summary>
        /// Calcular Total
        /// </summary>
        private void calcularTotal()
        {
            try
            {
                if (txtCantidad.Text.Trim() == "") txtTotalProducto.Text = "0";
                if (txtPrecioUnitario.Text.Trim() == "" || txtCantidad.Text.Trim() == "" || txtDescuento.Text.Trim() == "") return; /// Validación
                double precioUnidario =toDouble(txtPrecioUnitario.Text);
                double cantidad = toDouble(txtCantidad.Text);
                double descuento = toDouble(txtDescuento.Text);
                double totalConDescuento = (precioUnidario * cantidad) - (descuento / 100) * (precioUnidario * cantidad);
                txtTotalProducto.Text = darformato(totalConDescuento);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Calcular total", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        /// <summary>
        /// Calcular Precio Unitario
        /// </summary>
       


        #endregion
        private void cargarProductoDetalle()
        {
            if (cbxCodigoProducto.SelectedIndex == -1) return;
            try
            {
                /// Buscando el producto seleccionado
                int idProducto = Convert.ToInt32(cbxCodigoProducto.SelectedValue);
                currentProducto = listProductos.Find(x => x.idProducto == idProducto);
                cbxDescripcion.Text = currentProducto.codigoProducto;
                // Llenar los campos del producto escogido.............!!!!!
                txtCantidad.Text = "1";
                txtDescuento.Text = "0";
                /// Cargando presentaciones
                cargarPresentacionesProducto();
                /// Cargando alternativas del producto
                cargarAlternativasProducto();
                determinarDescuentoEImpuesto();
                determinarStock(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Listar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private  void cargarPresentacionesProducto()
        {

            // arreglar esto  no es necesrio ir a un servicio
            if (cbxCodigoProducto.SelectedIndex == -1) return; /// validacion   
            int idProducto = (int)cbxCodigoProducto.SelectedValue;
            ProductoVenta productoVenta=  listProductos.Find(X => X.idProducto == idProducto);
            /// Cargar las precentacione
            cbxDescripcion.SelectedValue =productoVenta.idPresentacion ;
            presentacionBindingSource.DataSource = listPresentacion;
            
        
        }
        private void calcularPrecioUnitarioProducto()
        {
            if (cbxCodigoProducto.SelectedIndex == -1) return; /// Validación
            try
            {
                if (cbxDescripcion.SelectedIndex == -1)
                {
                    txtPrecioUnitario.Text = currentProducto.precioVenta;
                }
                else
                {
                    // Buscar presentacion elegida
                    int idPresentacion = Convert.ToInt32(cbxDescripcion.SelectedValue);
                    Presentacion findPresentacion = listPresentacion.Find(x => x.idPresentacion == idPresentacion);

                    // Realizando el calculo
                    double precioCompra = toDouble(currentProducto.precioVenta);
                    double cantidadUnitario = toDouble(findPresentacion.cantidadUnitaria);
                    double precioUnidatio = precioCompra * cantidadUnitario;

                    // Imprimiendo valor
                    txtPrecioUnitario.Text = darformato(precioUnidatio);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Calcular total", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void cargarAlternativasProducto()
        {
            if (cbxCodigoProducto.SelectedIndex == -1) return; /// validacion
                                                               /// cargando las alternativas del producto
            List<AlternativaCombinacion> alternativaCombinacion = await alternativaModel.cAlternativa31(Convert.ToInt32(cbxCodigoProducto.SelectedValue));
            alternativaCombinacionBindingSource.DataSource = alternativaCombinacion;
            /// calculos
            calcularPrecioUnitarioProducto();
            calcularTotal();
        }

      
        // el calculo se hace entre las fechas  entre fechas
        public async void determinarDescuentoEImpuesto()
        {
            if (txtCantidad.Text != "" && cbxCodigoProducto.SelectedValue != null)
            {
                DescuentoProductoSubmit descuentoProductoSubmit = new DescuentoProductoSubmit();

                descuentoProductoSubmit.cantidad = toDouble(txtCantidad.Text);
                descuentoProductoSubmit.cantidades = "";
                descuentoProductoSubmit.idGrupoCliente = CurrentCliente != null ? CurrentCliente.idGrupoCliente : 1;
                descuentoProductoSubmit.idProducto = (int)cbxCodigoProducto.SelectedValue;
                descuentoProductoSubmit.idProductos = "";
                descuentoProductoSubmit.idSucursal = ConfigModel.sucursal.idSucursal;
                string dateEmision = String.Format("{0:u}", dtpEmision.Value);
                dateEmision = dateEmision.Substring(0, dateEmision.Length - 1);
                string dateVecimiento = String.Format("{0:u}", dtpFechaVecimiento.Value);
                dateVecimiento = dateVecimiento.Substring(0, dateVecimiento.Length - 1);
                descuentoProductoSubmit.fechaInicio = dateEmision;
                descuentoProductoSubmit.fechaFin = dateVecimiento;


                descuentoProducto = await descuentoModel.descuentototalentrefechas(descuentoProductoSubmit);

                txtDescuento.Text =darformato( descuentoProducto.descuento);

                calcularTotal();

                determinarDescuento();
                // para el descuento en grupo


            }
        }

        public async void determinarDescuento()
        {
            string dateEmision = String.Format("{0:u}", dtpEmision.Value);
            dateEmision = dateEmision.Substring(0, dateEmision.Length - 1);
            string dateVecimiento = String.Format("{0:u}", dtpFechaVecimiento.Value);
            dateVecimiento = dateVecimiento.Substring(0, dateVecimiento.Length - 1);


            if (detalleVentas != null)
                    if (detalleVentas.Count != 0)
                    {
                        //primero traemos los descuento correspondientes
                        DescuentoSubmit descuentoSubmit = new DescuentoSubmit();
                        string cantidades = "";
                        string idProductos = "";
                        foreach (DetalleV V in detalleVentas)
                        {
                            cantidades += V.cantidad + ",";
                            idProductos += V.idProducto + ",";
                        }

                        descuentoSubmit.idProductos = idProductos.Substring(0, idProductos.Length - 1);
                        descuentoSubmit.cantidades = cantidades.Substring(0, cantidades.Length - 1);
                        descuentoSubmit.idGrupoCliente = CurrentCliente != null ? CurrentCliente.idGrupoCliente : 1;
                        descuentoSubmit.idSucursal = ConfigModel.sucursal.idSucursal;
                        descuentoSubmit.fechaInicio = dateEmision;
                        descuentoSubmit.fechaFin = dateVecimiento;

                        List<DescuentoReceive> descuentoReceive = await descuentoModel.descuentoTotalALaFechaGrupo(descuentoSubmit);




                        int i = 0;
                        foreach (DetalleV V in detalleVentas)
                        {
                            double descuento=descuentoReceive[i++].descuento;

                            V.descuento = darformato(descuento);
                            // nuevo Precio unitario
                            double precioUnitario =toDouble(V.precioVentaReal);
                            double precioUnitarioDescuento = precioUnitario - (descuento / 100) * precioUnitario;
                            V.precioVenta = darformato(precioUnitarioDescuento);

                            double precioUnitarioI1 = precioUnitarioDescuento;

                            double porcentual = toDouble(V.Porcentual); 
                            double  efectivo  = toDouble(V.Efectivo);
                            if (porcentual != 0)
                            {
                                double datoaux = (porcentual / 100) + 1;
                                precioUnitarioI1 = precioUnitarioDescuento / datoaux;
                            }
                            double precioUnitarioImpuesto = precioUnitarioI1 - efectivo;
                            V.precioUnitario = darformato(precioUnitarioImpuesto);
                            V.total = darformato(precioUnitarioImpuesto * toDouble(V.cantidad));// utilizar para sacar el subtotal
                            V.totalGeneral = darformato(precioUnitarioDescuento * toDouble(V.cantidad));
                           
                            
                        }

                        i = 0;
                       
                        detalleVBindingSource.DataSource = detalleVentas;
                       

                        // Calculo de totales y subtotales
                        calculoSubtotal();
                        descuentoTotal();

                    }

        }




        private void descuentoTotal()
        {
            if (cbxTipoMoneda.SelectedValue == null)
                return;
            double descuentoTotal = 0;
            // calcular el descuento total
            foreach (DetalleV V in detalleVentas)
            {
                // calculamos el decuento para cada elemento
                double precioReal = toDouble(V.precioVentaReal);
                double cantidad = toDouble(V.cantidad);
                double total = precioReal * cantidad;
                double descuentoV = total -toDouble( V.totalGeneral);
                descuentoTotal += descuentoV;

            }
            this.Descuento = descuentoTotal;
            Moneda moneda = monedas.Find(X => X.idMoneda == (int)cbxTipoMoneda.SelectedValue);          
            

            lbDescuentoVentas.Text= moneda.simbolo + ". " + darformato(descuentoTotal);


        }

        public async void determinarStock(double cantidad)
        {


            // determinamos el stock del producto seleccionado
            List<StockReceive> stockReceive = await stockModel.getStockProductoByIdProductoIdCombinacionIdSucursal((int)cbxCodigoProducto.SelectedValue, cbxVariacion.SelectedValue == null ? 0 : (int)cbxVariacion.SelectedValue, ConfigModel.sucursal.idSucursal, PersonalModel.personal.idPersonal);
            double stockTotal = stockReceive[0].stock_total;
            double stockDetalle = 0;
            // si exite en el producto en lista detalle
            if (detalleVentas != null && (cbxDescripcion.SelectedIndex != -1))
            {
                foreach (DetalleV V in detalleVentas)
                {
                    if (V.idPresentacion == (int)cbxDescripcion.SelectedValue)
                    {
                        stockDetalle = toDouble( V.cantidad);
                    }

                }
            }
            // restamos si exite en detalles de venta

            
            if (cantidad == 0)
            {

                if (stockTotal == 0)
                {
                    lbStock.Text = "/0";
                }
                else
                {
                    stockTotal -= stockDetalle;
                    lbStock.Text = "/" + stockTotal.ToString();
                }
            }
            else
            {

                if (stockTotal == 0)
                {
                    lbStock.Text = "/0";
                }
                else
                    lbStock.Text = "/" + stockTotal.ToString();
            }



        }

        // comenzando eventos

        private void cbxCodigoProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarProductoDetalle();
        }

        private void cargarDescripcionDetalle()
        {

            if (cbxDescripcion.SelectedIndex == -1) return;
            try
            {
                /// Buscando el producto seleccionado
                int idPresentacion = Convert.ToInt32(cbxDescripcion.SelectedValue);
                Presentacion presentacion  = listPresentacion.Find(x => x.idPresentacion == idPresentacion);
                cbxCodigoProducto.SelectedValue = presentacion.idProducto;
                // Llenar los campos del producto escogido.............!!!!!
                txtCantidad.Text = "1";
                txtDescuento.Text = "0";
                /// Cargando presentaciones           

                /// Cargando alternativas del producto
                cargarAlternativasdescripcion();
                determinarDescuentoEImpuesto();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Listar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        

        private void calcularPrecioUnitarioDescripcion()
        {
            if (cbxDescripcion.SelectedIndex == -1) return; /// Validación
            try
            {
                if (cbxDescripcion.SelectedIndex == -1)
                {
                    txtPrecioUnitario.Text = currentProducto.precioVenta;
                }
                else
                {
                    // Buscar presentacion elegida
                    int idPresentacion = Convert.ToInt32(cbxDescripcion.SelectedValue);
                    Presentacion findPresentacion = listPresentacion.Find(x => x.idPresentacion == idPresentacion);

                    // Realizando el calculo
                    double precioCompra = toDouble(currentProducto.precioVenta);
                    double cantidadUnitario =toDouble(findPresentacion.cantidadUnitaria);
                    double precioUnidatio = precioCompra * cantidadUnitario;

                    // Imprimiendo valor
                    txtPrecioUnitario.Text =darformato(precioUnidatio);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Calcular total", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void cargarAlternativasdescripcion()
        {
            if (cbxDescripcion.SelectedIndex == -1) return; /// validacion
                                                            /// cargando las alternativas del producto
            List<AlternativaCombinacion> alternativaCombinacion = await alternativaModel.cAlternativa31(Convert.ToInt32(cbxDescripcion.SelectedValue));
            alternativaCombinacionBindingSource.DataSource = alternativaCombinacion;

            /// calculos
            calcularPrecioUnitarioDescripcion();
            calcularTotal();
        }


        private void cbxDescripcion_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarDescripcionDetalle();
        }
        // validaciones
        private void txtCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validator.isNumber(e);
        }

        private void txtPrecioUnitario_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validator.isDecimal(e, txtPrecioUnitario.Text);
        }

        private void txtDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validator.isDecimal(e, txtDescuento.Text);
        }

        private void txtTotalProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            Validator.isDecimal(e, txtTotalProducto.Text);
        }

        private void dgvDetalleCompra_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            

                int y = e.ColumnIndex;

                if (dgvDetalleOrdenCompra.Columns[y].Name == "acciones")
                {
                    if (dgvDetalleOrdenCompra.Rows.Count == 0)
                    {
                        MessageBox.Show("No hay un registro seleccionado", "eliminar", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (nuevo)
                    {
                        int index = dgvDetalleOrdenCompra.CurrentRow.Index; // Identificando la fila actual del datagridview
                        int idPresentacion = Convert.ToInt32(dgvDetalleOrdenCompra.Rows[index].Cells[0].Value); // obteniedo el idRegistro del datagridview
                        DetalleV aux = detalleVentas.Find(x => x.idPresentacion == idPresentacion);

                        dgvDetalleOrdenCompra.Rows.RemoveAt(index);

                        detalleVentas.Remove(aux);

                        calculoSubtotal();
                        calcularDescuento();
                    }
                    else
                    {
                        int index = dgvDetalleOrdenCompra.CurrentRow.Index;
                        int idPresentacion = Convert.ToInt32(dgvDetalleOrdenCompra.Rows[index].Cells[0].Value); // obteniedo el idRegistro del datagridview
                        DetalleV aux = detalleVentas.Find(x => x.idPresentacion == idPresentacion);

                        aux.estado = 9;
                        
                        dgvDetalleOrdenCompra.ClearSelection();
                        dgvDetalleOrdenCompra.Rows[index].DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 224);
                        dgvDetalleOrdenCompra.Rows[index].DefaultCellStyle.ForeColor = Color.FromArgb(250, 5, 73);

                        decorationDataGridView();
                        calculoSubtotal();
                        calcularDescuento();


                     }

                }

            
            
           


        }

        private void dgvDetalleCompra_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificando la existencia de datos en el datagridview
            if (dgvDetalleOrdenCompra.Rows.Count == 0)
            {
                MessageBox.Show("No hay un registro seleccionado", "Modificar", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int index = dgvDetalleOrdenCompra.CurrentRow.Index; // Identificando la fila actual del datagridview
            int idPresentacion = Convert.ToInt32(dgvDetalleOrdenCompra.Rows[index].Cells[0].Value); // obteniedo el idRegistro del datagridview
            DetalleV aux = detalleVentas.Find(x => x.idPresentacion == idPresentacion); // Buscando la registro especifico en la lista de registros
            cbxCodigoProducto.Text = aux.codigoProducto;
            cbxDescripcion.Text = aux.descripcion;
            cbxVariacion.Text = aux.nombreCombinacion;
            cbxDescripcion.Text = aux.nombrePresentacion;
            txtCantidad.Text =darformato( toDouble(aux.cantidad));
            txtPrecioUnitario.Text = darformato(aux.precioVentaReal);
            txtDescuento.Text = darformato( aux.descuento);
            txtTotalProducto.Text = darformato( aux.totalGeneral);
            btnAgregar.Enabled = false;
            btnModificar.Enabled = true;
            dgvDetalleOrdenCompra.Rows[index].Selected = true;
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvDetalleOrdenCompra.Rows.Count == 0)
            {
                MessageBox.Show("No hay un registro seleccionado", "Modificar", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            int index = dgvDetalleOrdenCompra.CurrentRow.Index; // Identificando la fila actual del datagridview
            int idPresentacion = Convert.ToInt32(dgvDetalleOrdenCompra.Rows[index].Cells[0].Value); // obteniedo el idRegistro del datagridview
            DetalleV aux = detalleVentas.Find(x => x.idPresentacion == idPresentacion);



            aux.cantidad = txtCantidad.Text.Trim(); 
            aux.cantidadUnitaria= txtCantidad.Text.Trim();
            double descuento = toDouble(txtDescuento.Text.Trim());
            aux.descuento = darformato(descuento);
            double precioUnitario = toDouble(txtPrecioUnitario.Text.Trim());

            aux.precioVentaReal = darformato(precioUnitario);
            double precioUnitarioDescuento = precioUnitario - (descuento / 100) * precioUnitario;
            aux.precioVenta = darformato(precioUnitarioDescuento);

            // si es que exite impuesto al producto 

            // impuestoProducto
            // calculamos lo impuesto posibles del producto
            double porcentual = toDouble(aux.Porcentual);
            double efectivo = toDouble(aux.Efectivo);                      
            double precioUnitarioI1 = precioUnitarioDescuento;
            if (porcentual != 0)
            {
                double datoaux = (porcentual / 100) + 1;
                precioUnitarioI1 = precioUnitarioDescuento / datoaux;
            }
            double precioUnitarioImpuesto = precioUnitarioI1 - efectivo;
            aux.precioUnitario = darformato(precioUnitarioImpuesto);
            aux.total = darformato(precioUnitarioImpuesto * toDouble(aux.cantidad));// utilizar para sacar el subtotal
            aux.totalGeneral = darformato(precioUnitarioDescuento * toDouble(aux.cantidad));//utilizar para sacar el suTotal 


            detalleVBindingSource.DataSource = null;
            detalleVBindingSource.DataSource = detalleVentas;
            dgvDetalleOrdenCompra.Refresh();
            calculoSubtotal();
            descuentoTotal();
            btnAgregar.Enabled = true;
            btnModificar.Enabled = false;
        }


        private async void btnAgregar_Click(object sender, EventArgs e)
        {



            if (txtPrecioUnitario.Text == "")
            {
                txtPrecioUnitario.Text = "0";
            }
            if (txtDescuento.Text == "")
            {

                txtDescuento.Text = "0";
            }
            if (txtCantidad.Text == "")
            {
                txtCantidad.Text = "0";
            }

            bool seleccionado = false;
            if (cbxCodigoProducto.SelectedValue != null)
                seleccionado = true;
            if (cbxDescripcion.SelectedValue != null)
                seleccionado = true;
            // if(idProducto)


            if (seleccionado)
            {
                if (detalleVentas == null) detalleVentas = new List<DetalleV>();
                DetalleV detalleV = new DetalleV();

                DetalleV aux = detalleVentas.Find(X => X.idPresentacion == (int)cbxDescripcion.SelectedValue);
                if (aux != null)
                {

                    MessageBox.Show("Este dato ya fue agregado", "presentacion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;

                }



                // Creando la lista
                detalleV.cantidad = darformato(txtCantidad.Text.Trim());//1

                //determinamos el stock
                determinarStock(0);
                /// Busqueda presentacion
                Presentacion findPresentacion = listPresentacion.Find(x => x.idPresentacion == Convert.ToInt32(cbxDescripcion.SelectedValue));

                detalleV.idDetalleCotizacion=0;
                detalleV.idCotizacion = currentCotizacion!=null ? currentCotizacion.idCotizacion :0;// depende luego 
                detalleV.cantidadUnitaria = darformato(txtCantidad.Text.Trim());
                detalleV.codigoProducto = cbxCodigoProducto.Text.Trim();
                detalleV.descripcion = findPresentacion.descripcion;

                double descuento = toDouble(txtDescuento.Text.Trim());
                detalleV.descuento = darformato(descuento);
                double precioUnitario = toDouble(txtPrecioUnitario.Text.Trim());

                detalleV.precioVentaReal = darformato(precioUnitario);
                double precioUnitarioDescuento = precioUnitario - (descuento / 100) * precioUnitario;
                detalleV.precioVenta = darformato(precioUnitarioDescuento);

                // si es que exite impuesto al producto 

                // impuestoProducto
                listImpuestosProducto = await impuestoModel.impuestoProducto(findPresentacion.idProducto, ConfigModel.sucursal.idSucursal);
                // calculamos lo impuesto posibles del producto
                double porcentual = 0;
                double efectivo = 0;
                foreach (ImpuestoProducto I in listImpuestosProducto)
                {
                    if (I.porcentual)
                    {
                        porcentual += toDouble(I.valorImpuesto);
                    }
                    else
                    {
                        efectivo += toDouble(I.valorImpuesto);
                    }
                }
                detalleV.Porcentual =darformato( porcentual);
                detalleV.Efectivo = darformato(efectivo);

                double precioUnitarioI1 = precioUnitarioDescuento;
                if (porcentual != 0)
                {
                    double datoaux = (porcentual / 100) + 1;
                    precioUnitarioI1 = precioUnitarioDescuento / datoaux;
                }
                double precioUnitarioImpuesto = precioUnitarioI1 - efectivo;
                detalleV.precioUnitario = darformato(precioUnitarioImpuesto);
                detalleV.total = darformato(precioUnitarioImpuesto * toDouble(detalleV.cantidad));// utilizar para sacar el subtotal
                detalleV.totalGeneral = darformato(precioUnitarioDescuento * toDouble(detalleV.cantidad));//utilizar para sacar el suTotal
                detalleV.valor = darformato(toDouble(detalleV.totalGeneral) - toDouble(detalleV.total));
                // fin cde calculo de necesarios par detalles productos



                detalleV.estado = 1;//5
                detalleV.idCombinacionAlternativa = Convert.ToInt32(cbxVariacion.SelectedValue);//7
                detalleV.idPresentacion = Convert.ToInt32(cbxDescripcion.SelectedValue);
                detalleV.idProducto = Convert.ToInt32(cbxCodigoProducto.SelectedValue);
                detalleV.idSucursal = ConfigModel.sucursal.idSucursal;
                detalleV.nombreCombinacion = cbxVariacion.Text;
                detalleV.nombreMarca = currentProducto.nombreMarca;
                detalleV.nombrePresentacion = cbxDescripcion.Text;
                detalleV.nro = 1;
                // determinar el impuesto                 

                detalleV.eliminar = "";
                int scotk = 0;
                if (lbStock.Text != "")
                {
                    scotk = Convert.ToInt32(lbStock.Text.Substring(1, lbStock.Text.Length - 1));

                }

                detalleV.existeStock = (scotk > 0 && scotk >= Convert.ToInt32(txtCantidad.Text.Trim())) ? 1 : 0;             
                ProductoVenta aux1 = listProductos.Find(x => x.idProducto == (int)cbxCodigoProducto.SelectedValue);
                detalleV.nombreMarca = aux1.nombreMarca;
                detalleV.nombrePresentacion = findPresentacion.nombrePresentacion;
                detalleV.precioEnvio = "";
                detalleV.ventaVarianteSinStock = aux1.ventaVarianteSinStock;
                // agrgando un nuevo item a la lista
                detalleVentas.Add(detalleV);

                // calcular los descuentos


                // Refrescando la tabla
                detalleVBindingSource.DataSource = null;
                detalleVBindingSource.DataSource = detalleVentas;
                dgvDetalleOrdenCompra.Refresh();

                // Calculo de totales y subtotales
                calculoSubtotal();

                descuentoTotal();

                limpiarCamposProducto();

            }
            else
            {

                MessageBox.Show("Error: elemento no seleccionado", "agregar Elemento", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }



          


        }


        
       

        private void txtCantidad_TextChanged(object sender, EventArgs e)
        {
            determinarDescuentoEImpuesto();
        }

        private void txtPrecioUnitario_TextChanged(object sender, EventArgs e)
        {
            calcularTotal();
        }

        private void txtDescuento_TextChanged(object sender, EventArgs e)
        {
            calcularTotal();
        }

        


        private void calcularDescuento()
        {



            if (cbxTipoMoneda.SelectedValue == null)
                return;

            double descuentoTotal = 0;
            Moneda moneda = monedas.Find(X => X.idMoneda == (int)cbxTipoMoneda.SelectedValue);

            // calcular el descuento total
            foreach (DetalleV V in detalleVentas)
            {
                // calculamos el decuento para cada elemento

                if (V.estado == 1)
                {
                    double total = toDouble( V.precioUnitario) * toDouble( V.cantidad);
                    double descuentoC = total - toDouble(V.total);
                    descuentoTotal += descuentoC;
                }
                   
            }
            this.Descuento = descuentoTotal;

            lbDescuentoVentas.Text = moneda.simbolo + ". " + darformato(descuentoTotal);

        }

        
        private bool exitePresentacion(int idPresentacion)
        {
            foreach (DetalleV C in detalleVentas)
            {
                if (C.idPresentacion == idPresentacion)
                    return true;
            }

            return false;

        }

        private void decorationDataGridView()
        {
            if (dgvDetalleOrdenCompra.Rows.Count == 0) return;

            foreach (DataGridViewRow row in dgvDetalleOrdenCompra.Rows)
            {
                int idPresentacion = Convert.ToInt32(row.Cells[0].Value); // obteniedo el idCategoria del datagridview

                DetalleV aux = detalleVentas.Find(x => x.idPresentacion == idPresentacion); // Buscando la categoria en las lista de categorias
                if (aux.estado == 0 || aux.estado==9)
                {
                    dgvDetalleOrdenCompra.ClearSelection();
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 224, 224);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(250, 5, 73);
                }
            }
        }
      

        private void limpiarCamposProducto()
        {
            cbxCodigoProducto.SelectedIndex = -1;
            cbxDescripcion.SelectedIndex = -1;

            cbxVariacion.SelectedIndex = -1;
            txtCantidad.Text = "";
            txtDescuento.Text = "";
            txtPrecioUnitario.Text = "";
            txtTotalProducto.Text = "";
        }

        private async void btnComprar_Click(object sender, EventArgs e)
        {
            cotizacionG.correlativo = txtCorrelativo.Text.Trim();
            cotizacionG.descuento = darformatoGuardar(this.Descuento);
            cotizacionG.direccion = txtDireccionCliente.Text.Trim();
            cotizacionG.documentoIdentificacion = cbxTipoDocumento.Text;
            cotizacionG.editar =currentCotizacion!=null ? false: chbxEditar.Checked;
            cotizacionG.estado = 1;

            string fechaEmision = String.Format("{0:u}", dtpEmision.Value);
            fechaEmision = fechaEmision.Substring(0, fechaEmision.Length - 1);
            string fechaVencimiento = String.Format("{0:u}", dtpFechaVecimiento.Value);
            fechaVencimiento = fechaVencimiento.Substring(0, fechaVencimiento.Length - 1);


            cotizacionG.fechaEmision = fechaEmision;

            cotizacionG.fechaVencimiento = fechaVencimiento;

            Cliente cliente = listClientes.Find(X => X.idCliente == (int)cbxCliente.SelectedValue);

            cotizacionG.idCliente = cliente.idCliente;
            cotizacionG.idCotizacion = currentCotizacion != null ? currentCotizacion.idCotizacion : 0; // ver en modificar
            cotizacionG.idDocumentoIdentificacion = (int )cbxTipoDocumento.SelectedValue;
            cotizacionG.idGrupoCliente = cliente.idGrupoCliente;
            cotizacionG.idMoneda = (cbxTipoMoneda.SelectedItem as Moneda).idMoneda;
            cotizacionG.idPersonal = PersonalModel.personal.idPersonal;
            cotizacionG.idSucursal = ConfigModel.sucursal.idSucursal;
            cotizacionG.idTipoDocumento = 2; // COTIZACION
            cotizacionG.moneda = (cbxTipoMoneda.SelectedItem as Moneda).moneda;
            cotizacionG.nombreCliente = cliente.nombreCliente;
            cotizacionG.observacion = txtObservaciones.Text.Trim();
            cotizacionG.personal = PersonalModel.personal.nombres;
            cotizacionG.rucDni = cliente.numeroDocumento;
            cotizacionG.serie = txtSerie.Text.Trim();
            cotizacionG.subTotal =darformatoGuardar( subTotal);
            cotizacionG.tipoCambio = 1;
            cotizacionG.total = darformatoGuardar(total);


            foreach(DetalleV V in  detalleVentas)
            {

                V.descuento = V.descuento.Replace(",","");
                V.precioUnitario = V.precioUnitario.Replace(",", "");
                V.total = V.total.Replace(",", "");
                V.precioVenta = V.precioVenta.Replace(",", "");
                V.precioVentaReal = V.precioVentaReal.Replace(",", "");
                V.totalGeneral = V.totalGeneral.Replace(",", "");
                V.valor = V.valor.Replace(",", "");
            }

            totalCotizacion.cotizacion = cotizacionG;
            totalCotizacion.detalle = detalleVentas;

           

            Response response= await  cotizacionModel.guardar(totalCotizacion);
          
            if (response.id>0)
            {
                if(nuevo)
                {
                    MessageBox.Show(response.msj, "Guardar", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
                else
                {
                    MessageBox.Show(response.msj, "Modificar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
               

            }
         
                           
        }

        private void Observaciones_Click(object sender, EventArgs e)
        {

        }

     

       
        private void cbxProveedor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxCliente.SelectedIndex == -1) return;

            CurrentCliente = listClientes.Find(X => X.idCliente == (int)cbxCliente.SelectedValue);

            datosClientes();

            determinarDescuento();
        }



        private void btnModificar_EnabledChanged(object sender, EventArgs e)
        {
            if(btnModificar.Enabled)
                this.btnModificar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(103)))), ((int)(((byte)(178)))));
            else
                btnModificar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(88)))), ((int)(((byte)(152)))));       
        }

        private void btnAgregar_EnabledChanged(object sender, EventArgs e)
        {

            if(btnModificar.Enabled)
                this.btnAgregar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            else
                
                btnAgregar.BackColor= System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(139)))), ((int)(((byte)(23)))));

        }

        private void btnActulizar_Click(object sender, EventArgs e)
        {            
            cargarProductos();
                    
        }

        private void btnNuevoProducto_Click(object sender, EventArgs e)
        {
            FormProductoNuevo formProductoNuevo = new FormProductoNuevo();
            formProductoNuevo.ShowDialog();
            cargarProductos();
           
        }

        private void txtDni_TextChanged(object sender, EventArgs e)
        {
            String aux = txtDni.Text;

            int nroCaracteres = aux.Length;
            bool exiteProveedor = false;
            if (nroCaracteres == 11 || nroCaracteres ==8)
            {
                try
                {                    
                    CurrentCliente = listClientes.Find(X=>X.numeroDocumento ==aux);
                    if (CurrentCliente!=null)
                    {                
                        exiteProveedor = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "buscar cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (exiteProveedor)
                {
                    // llenamos los dato con el current proveerdor
                    datosClientes();
                }
                else
                {
                    //llenamos los datos en FormproveerdorNuevo
                    FormClienteNuevo formClienteNuevo = new FormClienteNuevo(aux);
                    formClienteNuevo.ShowDialog();
                    cargarClientes();
                    Response response = formClienteNuevo.uCClienteGeneral.rest;
                    if (response != null)
                        if (response.id > 0)
                        {
                            CurrentCliente = listClientes.Find(X => X.idCliente == response.id);
                            datosClientes();
                        }
                }
            }

        }



        private void  datosClientes()
        {
            if (CurrentCliente != null)
            {

                txtDni.removePlaceHolder();
                txtDni.Text = CurrentCliente.numeroDocumento;
                txtDireccionCliente.Text = CurrentCliente.direccion;
                cbxCliente.Text = CurrentCliente.nombreCliente;
                cbxTipoDocumento.SelectedValue = CurrentCliente.idDocumento;

            }
            
        }

        private void executeBuscarCliente()
        {
            Buscarcliente buscarCliente = new Buscarcliente();
            buscarCliente.ShowDialog();
            this.CurrentCliente = buscarCliente.currentCliente;
            datosClientes();
            //determinarDescuento();
        }

      

        

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            executeBuscarCliente();
        }

        private void chbxNotaEntrada_OnChange(object sender, EventArgs e)
        {
            if (!txtCorrelativo.Enabled)
                txtCorrelativo.Enabled = true;
            else
            {
                txtCorrelativo.Enabled = false;
            }
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
