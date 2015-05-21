using Microsoft.Practices.Prism.Mvvm;
using PatientControl.Models;
using SQLite;
using System;
using System.Collections.Generic;
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
        public int Paciente_Id { get { return _ejercicio.paciente_id; } set { _ejercicio.paciente_id = value; } }
        public string Angulo { get { return _ejercicio.angulo; } set { _ejercicio.angulo = value; } }
        public int Repeticiones { get { return _ejercicio.repeticiones; } set { _ejercicio.repeticiones = value; } }
        public DateTime FechaRealizado { get { return _ejercicio.fechaRealizado; } set { _ejercicio.fechaRealizado = value; } }


        /*public string AnguloAb { get { return _ejercicio.anguloAb; } set { _ejercicio.anguloAb = value; } }
        public string AnguloAd { get { return _ejercicio.anguloAd; } set { _ejercicio.anguloAd = value; } }
        public int RepeticionesAbd { get { return _ejercicio.repeticionesAbd; } set { _ejercicio.repeticionesAbd = value; } }
        public string AnguloFl { get { return _ejercicio.anguloFl; } set { _ejercicio.anguloFl = value; } }
        public string AnguloEx { get { return _ejercicio.anguloEx; } set { _ejercicio.anguloEx = value; } }
        public int RepeticionesFlEx { get { return _ejercicio.repeticionesFlEx; } set { _ejercicio.repeticionesFlEx = value; } }
        public string AnguloFlh { get { return _ejercicio.anguloFlh; } set { _ejercicio.anguloFlh = value; } }
        public string AnguloExh { get { return _ejercicio.anguloExh; } set { _ejercicio.anguloExh = value; } }
        public int RepeticionesFlExh { get { return _ejercicio.repeticionesFlExh; } set { _ejercicio.repeticionesFlExh = value; } }
        public string AnguloCfl { get { return _ejercicio.anguloCfl; } set { _ejercicio.anguloCfl = value; } }
        public string AnguloCex { get { return _ejercicio.anguloCex; } set { _ejercicio.anguloCex = value; } }
        public int RepeticionesCflEx { get { return _ejercicio.repeticionesCflEx; } set { _ejercicio.repeticionesCflEx = value; } }*/

        public double Diferencia { get { return _ejercicio.diferencia; } set { _ejercicio.diferencia = value; } }

        public int ItemPosition { get; set; }

        public async Task<List<EjercicioViewModel>> ObtenerEjercicios(string data){
            List<EjercicioViewModel> lista = new List<EjercicioViewModel>();
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            var result = await connection.QueryAsync<Ejercicio>("Select * FROM Ejercicios WHERE paciente_id = ?", new object[] { data });
            if (result.Count() == 0) return null;
            foreach (var item in result)
            {
                this.Articulacion = item.articulacion;
                this.Id = item.id;
                this.Tipo = item.tipo;
                this.Paciente_Id = item.paciente_id;
                this.FechaRealizado = item.fechaRealizado;
                this.Angulo = item.angulo;
                this.Repeticiones = item.repeticiones;
                /*this.AnguloAb = item.anguloAb;
                this.AnguloAd { get { return _ejercicio.anguloAd;
                this.RepeticionesAbd = item.repeticionesAbd;
                this.AnguloFl = item.anguloFl;
                this.AnguloEx = item.anguloEx;
                this.RepeticionesFlEx = item.repeticionesFlEx;
                this.AnguloFlh = item.anguloFlh;
                this.AnguloExh = item.anguloExh;
                this.RepeticionesFlExh = item.repeticionesFlExh;
                this.nguloCfl = item.anguloCfl;
                this.AnguloCex = item.anguloCex;
                this.RepeticionesCflEx = item.repeticionesCflEx;*/
                this.Diferencia = item.diferencia;
                lista.Add(this);
            }
            return lista;
        }

        public async void CreateDatabase()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            await connection.CreateTableAsync<Ejercicio>();
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
