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

        public string Title { get { return _ejercicio.titulo; } set { _ejercicio.titulo = value; } }

        public string Description { get { return _ejercicio.descripcion; } set { _ejercicio.descripcion = value; } }

        public int Id { get { return _ejercicio.id; } }

        public int Paciente_Id { get { return _ejercicio.paciente_id; } set { _ejercicio.paciente_id = value; } }

        public string AnguloAb { get { return _ejercicio.anguloAb; } set { _ejercicio.anguloAb = value; } }
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
        public int RepeticionesCflEx { get { return _ejercicio.repeticionesCflEx; } set { _ejercicio.repeticionesCflEx = value; } }

        public Uri Image { get { return _ejercicio.imageUri; } set { _ejercicio.imageUri = value; } }

        public int ItemPosition { get; set; }

        public async Task<bool> ObtenerEjercicioInfo(string data){
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            var result = await connection.QueryAsync<Ejercicio>("Select * FROM Ejercicios WHERE paciente_id = ?", new object[] { data });
            if (result.Count() == 0) return false;
            foreach (var item in result)
            {
                /*this.Id = item.id;
                this.Nombre = item.nombre;
                this.Apellidos = item.apellidos;
                this.Direccion = item.direccion;
                this.DireccionOpcional = item.direccionOpcional;
                this.Localidad = item.localidad;
                this.Provincia = item.provincia;
                this.CodPostal = item.codigoPostal;
                this.Telefono = item.telefono;*/
            }
            return true;
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
