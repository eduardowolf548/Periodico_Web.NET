﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntidadesCompartidas;
using System.Data.SqlClient;
using System.Data;
namespace Persistencia
{
    public class PersistenciaNacionales
    {
        public static void Alta(Nacionales unaN)
        {
            SqlConnection _Conexion = new SqlConnection(Conexion._Cnn);
            SqlCommand _Comando = new SqlCommand("AgregarNoticiaNacional", _Conexion);
            _Comando.CommandType = CommandType.StoredProcedure;

            _Comando.Parameters.AddWithValue("@FechaHora", unaN.FechaHora);
            _Comando.Parameters.AddWithValue("@Resumen", unaN.Resumen);
            _Comando.Parameters.AddWithValue("@Contenido", unaN.Contenido);
            _Comando.Parameters.AddWithValue("@Titulo", unaN.Titulo);
            _Comando.Parameters.AddWithValue("@CodigoReg", unaN.Periodista.CodigoReg);
            _Comando.Parameters.AddWithValue("@CodIntS", unaN.Secciones.CodIntS);
      
            SqlParameter _Retorno = new SqlParameter("@Retorno", SqlDbType.Int);
            _Retorno.Direction = ParameterDirection.ReturnValue;
            _Comando.Parameters.Add(_Retorno);

            try
            {
                _Conexion.Open();
                _Comando.ExecuteNonQuery();

                int oAfectados = (int)_Comando.Parameters["@Retorno"].Value;

                if (oAfectados == -1)
                    throw new Exception("No existe el Periodista");
                else if (oAfectados == -2)
                    throw new Exception("No existe la Sección");
                else if (oAfectados == -3)
                    throw new Exception("Error en TRN");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _Conexion.Close();
            }
        }

        public static List<Nacionales> ListarNN()
        {
            int pNroInt;
            DateTime pFechaHora;
            String pResumen, pContenido, pTitulo;
            int pCodigoReg;
            string pCodIntS;
            Secciones pSecciones;
            Periodista pPeriodista;
            Nacionales n;
            List<Nacionales> _lista = new List<Nacionales>(); 

            SqlDataReader _Reader;
            SqlConnection _Conexion = new SqlConnection(Conexion._Cnn);
            SqlCommand _Comando = new SqlCommand("ListarNoticiasNacionales", _Conexion);
            _Comando.CommandType = CommandType.StoredProcedure;

            try
            {
                _Conexion.Open();
                _Reader = _Comando.ExecuteReader();

                while (_Reader.Read())
                {
                    pNroInt = (int)_Reader["NroInt"];
                    pFechaHora = (DateTime)_Reader["FechaHora"];
                    pResumen = (string)_Reader["Resumen"];
                    pContenido = (string)_Reader["Contenido"];
                    pTitulo = (string)_Reader["Titulo"];
                    pCodigoReg = (int)_Reader["CodigoReg"];
                    pCodIntS = (string)_Reader["CodIntS"];

                    pSecciones = PersisitenciaSecciones.Buscar(pCodIntS);
                    pPeriodista = PersistenciaPeriodista.Buscar(pCodigoReg);

                    n = new Nacionales(pFechaHora, pResumen, pContenido, pTitulo, pSecciones, pPeriodista);
                    _lista.Add(n);
                }
                

                _Reader.Close();

            }
            catch (Exception ex)
            {
                throw new Exception("Problemas con la base de datos:" + ex.Message);
            }
            finally
            {
                _Conexion.Close();
            }

            return _lista;
        }




        public static List<Noticias> ListarSeccionesNoticias(Secciones unaS)
        {
            List<Noticias> _lista = new List<Noticias>(); 
            SqlDataReader _Reader;

            SqlConnection _Conexion = new SqlConnection(Conexion._Cnn);
            SqlCommand _Comando = new SqlCommand("ListarSeccionesNoticias", _Conexion);
            _Comando.CommandType = CommandType.StoredProcedure;

            _Comando.Parameters.AddWithValue("@CodIntS", unaS.CodIntS);

            try
            {
                _Conexion.Open();
                _Reader = _Comando.ExecuteReader();

                if (_Reader.HasRows)
                {
                    while (_Reader.Read())
                    {
                        Nacionales N = new Nacionales(Convert.ToDateTime(_Reader["FechaHora"]), _Reader["Resumen"].ToString(), _Reader["Contenido"].ToString(), _Reader["Titulo"].ToString(), PersisitenciaSecciones.Buscar(_Reader["CodIntS"].ToString()), PersistenciaPeriodista.Buscar(Convert.ToInt32(_Reader["CodigoReg"])));
                        _lista.Add(N);
                    }
                }

                _Reader.Close();

            }
            catch (Exception ex)
            {
                throw new ApplicationException("Problemas con la base de datos:" + ex.Message);
            }
            finally
            {
                _Conexion.Close();
            }

            return _lista;
        }
    }
}
