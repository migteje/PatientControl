using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using PatientControl.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientControl.ViewModels
{
    public class EjercicioViewModel : BindableBase
    {
        private readonly Ejercicio _ejercicio;

        public EjercicioViewModel(Ejercicio ejercicio)
        {
            if (ejercicio == null) throw new ArgumentNullException("ejercicio");
            _ejercicio = ejercicio;
        }

        public int Id { get { return _ejercicio.id; } set { _ejercicio.id = value; } }

        public string Title { get { return _ejercicio.titulo; } set { _ejercicio.titulo = value; } }
        public string Description { get { return _ejercicio.descripcion; } set { _ejercicio.descripcion = value; } }
        public Uri Image { get { return _ejercicio.imageUri; } set { _ejercicio.imageUri = value; } }

        public string Articulacion { get { return _ejercicio.articulacion; } set { _ejercicio.articulacion = value; } }
        public string Tipo { get { return _ejercicio.tipo; } set { _ejercicio.tipo = value; } }
        public string Lado { get { return _ejercicio.lado; } set { _ejercicio.lado = value; } }
        public int Paciente_Id { get { return _ejercicio.paciente_id; } set { _ejercicio.paciente_id = value; } }
        public string Angulo { get { return _ejercicio.angulo; } set { _ejercicio.angulo = value; } }
        public int Repeticiones { get { return _ejercicio.repeticiones; } set { _ejercicio.repeticiones = value; } }
        public DateTime FechaRealizado { get { return _ejercicio.fechaRealizado; } set { _ejercicio.fechaRealizado = value; } }

        public double Diferencia { get { return _ejercicio.diferencia; } set { _ejercicio.diferencia = value; } }

        public int ItemPosition { get; set; }

        public async Task<List<EjercicioViewModel>> ObtenerEjercicios(string data, string selected){
            List<EjercicioViewModel> list = new List<EjercicioViewModel>();
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            string query = "";
            if (selected.Equals("todas"))
                query = "Select * FROM Ejercicios WHERE paciente_id = ?";
            else
                query = "Select * FROM Ejercicios WHERE paciente_id = ? AND tipo LIKE '"+selected+"%'";
            var result = await connection.QueryAsync<Ejercicio>(query, new object[] { data });
            if (result.Count() == 0) return null;

            foreach (var item in result)
            {
                EjercicioViewModel ej = new EjercicioViewModel(new Ejercicio());
                ej.Articulacion = item.articulacion;
                ej.Id = item.id;
                ej.Tipo = item.tipo;
                ej.Paciente_Id = item.paciente_id;
                ej.FechaRealizado = item.fechaRealizado;
                ej.Angulo = item.angulo;
                ej.Repeticiones = item.repeticiones;
                ej.Diferencia = item.diferencia;
                list.Add(ej);
            }
                return list;
        }

        public async Task InsertarNuevoEjercicio()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            await connection.InsertAsync(this._ejercicio);
        }

        public async Task UpdateEjercicioInfo(String name)
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            var Ejer = await connection.Table<Ejercicio>().Where(x => x.paciente_id.Equals(name)).FirstOrDefaultAsync();

            if (Ejer != null)
            {
                /*Patient.diagnostico = this.Diagnostico;
                Patient.zonaLesion = this.ZonaLesion;
                Patient.fechaLesion = this.FechaLesion;
                Patient.articValorar = this.ArticValorar;
                Patient.bodyLeftRight = this.BodyLeftRight;*/

                await connection.UpdateAsync(Ejer);
            }
        }
    }
}
