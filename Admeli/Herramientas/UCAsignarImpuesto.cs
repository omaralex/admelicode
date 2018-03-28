﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Admeli.Componentes;
using Modelo;
using Entidad;

namespace Admeli.Herramientas
{
    public partial class UCAsignarImpuesto : UserControl
    {
        private FormPrincipal formPrincipal;
        public bool lisenerKeyEvents { get; set; }

        private ProductoModel productoModel = new ProductoModel();
        private ImpuestoModel impuestoModel = new ImpuestoModel();
        List<ImpuestosSiglas> listImpuestos;
        List<ProductoSinImpuesto> listProductos;
        public UCAsignarImpuesto()
        {
            InitializeComponent();
        }

        public UCAsignarImpuesto(FormPrincipal formPrincipal)
        {
            InitializeComponent();
            this.formPrincipal = formPrincipal;

            lisenerKeyEvents = true; // Active lisener key events
        }

        private void panelContainer_Paint(object sender, PaintEventArgs e)
        {
            DrawShape drawShape = new DrawShape();
            drawShape.lineBorder(panelContainer);
        }

        #region ============================== Root Load ==============================
        private void UCAsignarImpuesto_Load(object sender, EventArgs e)
        {
            this.reLoad();
        }

        internal void reLoad(bool refreshData = true)
        {
            if (refreshData)
            {
                cargarProductos();
                cargarImpuestos();
            }
            lisenerKeyEvents = true; // Active lisener key events
        }
        #endregion

        #region ====================================== Loads ======================================
        private async void cargarProductos()
        {
            loadState(true);
            try
            {
                /// categoriaBindingSource.DataSource = await categoriaModel.categorias21();
                /// 
                listProductos = await productoModel.listarProductoPorIdProductoCodigoNombreSinImpuesto(ConfigModel.sucursal.idSucursal);
                productoSinImpuestoBindingSource.DataSource = listProductos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Listar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                loadState(false);
            }
        }

        private  async void cargarImpuestos()
        {
            loadState(true);
            try
            {
                listImpuestos = await impuestoModel.listarImpuestoIdImpuestoNombreSiglasByActivos();
                impuestosSiglasBindingSource.DataSource = listImpuestos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Listar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                loadState(false);
            }
        }
        #endregion

        #region ==================================== Estados ====================================
        private void loadState(bool state)
        {
            formPrincipal.appLoadState(state);
        }
        #endregion

    }
}
