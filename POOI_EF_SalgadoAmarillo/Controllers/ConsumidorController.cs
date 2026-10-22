using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using POOI_EF_SalgadoAmarillo.Models;

namespace POOI_EF_SalgadoAmarillo.Controllers
{
    public class ConsumidorController : Controller
    {
        private string cadena = ConfigurationManager.ConnectionStrings["ExamenFinal"].ConnectionString;

        private Consumidor LeerConsumidor(SqlDataReader lector)
        {
            return new Consumidor
            {
                dnicont = lector["dnicont"].ToString(),
                nomcont = lector["nomcont"].ToString(),
                apecont = lector["apecont"].ToString(),
                emailcont = lector["emailcont"].ToString(),
                idpais = lector["idpais"].ToString()
            };
        }

        private List<Consumidor> Listar()
        {
            List<Consumidor> lista = new List<Consumidor>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_listar_consumidor", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader lector = cmd.ExecuteReader())
                {
                    while (lector.Read())
                        lista.Add(LeerConsumidor(lector));
                }
            }
            return lista;
        }

        private Consumidor Buscar(string dni)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_detalle_consumidor", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@dnicont", SqlDbType.VarChar, 50).Value = dni ?? "";
                cn.Open();
                using (SqlDataReader lector = cmd.ExecuteReader())
                {
                    if (lector.Read())
                        return LeerConsumidor(lector);
                }
            }
            return null;
        }

        private void CargarPaises(string seleccionado = null)
        {
            List<SelectListItem> paises = new List<SelectListItem>();
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_listar_paises", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                using (SqlDataReader lector = cmd.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        paises.Add(new SelectListItem
                        {
                            Value = lector["idpais"].ToString(),
                            Text = lector["nombrepais"].ToString(),
                            Selected = lector["idpais"].ToString() == seleccionado
                        });
                    }
                }
            }
            ViewBag.Paises = paises;
        }

        private string Guardar(Consumidor consumidor, string procedimiento)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand(procedimiento, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@dnicont", SqlDbType.VarChar, 50).Value = consumidor.dnicont;
                cmd.Parameters.Add("@nomcont", SqlDbType.VarChar, 100).Value = consumidor.nomcont;
                cmd.Parameters.Add("@apecont", SqlDbType.VarChar, 100).Value = consumidor.apecont;
                cmd.Parameters.Add("@emailcont", SqlDbType.VarChar, 300).Value = (object)consumidor.emailcont ?? DBNull.Value;
                cmd.Parameters.Add("@idpais", SqlDbType.VarChar, 20).Value = consumidor.idpais;
                cmd.Parameters.Add("@mensaje", SqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
                cn.Open();
                cmd.ExecuteNonQuery();
                return cmd.Parameters["@mensaje"].Value.ToString();
            }
        }

        public ActionResult Index()
        {
            return View(Listar());
        }

        public ActionResult Create()
        {
            CargarPaises();
            return View(new Consumidor());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Consumidor consumidor)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Mensaje = Guardar(consumidor, "usp_agrega_consumidor");
                if (ViewBag.Mensaje == "Consumidor registrado correctamente.")
                {
                    ModelState.Clear();
                    consumidor = new Consumidor();
                }
            }
            CargarPaises(consumidor.idpais);
            return View(consumidor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string dni)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("usp_eliminar_consumidor", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@dnicont", SqlDbType.VarChar, 50).Value = dni ?? "";
                cmd.Parameters.Add("@mensaje", SqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
                cn.Open();
                cmd.ExecuteNonQuery();
                TempData["Mensaje"] = cmd.Parameters["@mensaje"].Value.ToString();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Update(string dni)
        {
            Consumidor consumidor = Buscar(dni);
            if (consumidor == null)
            {
                TempData["Mensaje"] = "No se encontró el consumidor.";
                return RedirectToAction("Index");
            }
            CargarPaises(consumidor.idpais);
            return View(consumidor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Consumidor consumidor)
        {
            if (ModelState.IsValid)
                ViewBag.Mensaje = Guardar(consumidor, "usp_actualiza_consumidor");

            CargarPaises(consumidor.idpais);
            return View(consumidor);
        }

        public ActionResult Details(string dni)
        {
            Consumidor consumidor = Buscar(dni);
            ViewBag.Mensaje = consumidor == null ? "No se encontró el consumidor." : "Consumidor encontrado.";
            return View(consumidor);
        }
    }
}
