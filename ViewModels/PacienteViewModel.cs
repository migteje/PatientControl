using Microsoft.Practices.Prism.Mvvm;
using PatientControl.Models;
using PatientControl.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PatientControl.ViewModels
{
    public class PacienteViewModel: BindableBase
    {
        private readonly Paciente _paciente;
        public PacienteViewModel(Paciente paciente)
        {
            if (paciente == null) throw new ArgumentNullException("paciente");
            _paciente = paciente;
        }

        public int Id
        {
            get { return _paciente.id; }
            set { _paciente.id = value; }
        }

        public string Nombre
        {
            get { return _paciente.nombre; }
            set { _paciente.nombre = value; }
        }

        public string Apellidos
        {
            get { return _paciente.apellidos; }
            set { _paciente.apellidos = value; }
        }

        public string Direccion
        {
            get { return _paciente.direccion; }
            set { _paciente.direccion = value; }
        }

        public string Provincia
        {
            get { return _paciente.provincia; }
            set { _paciente.provincia = value; }
        }

        public string Localidad
        {
            get { return _paciente.localidad; }
            set { _paciente.localidad = value; }
        }

        [StringLength(6, MinimumLength = 5, ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "ZipCodeLengthInvalidErrorMessage")]
        public string CodPostal
        {
            get { return _paciente.codigoPostal; }
            set { _paciente.codigoPostal = value; }
        }

        public string Telefono
        {
            get { return _paciente.telefono; }
            set { _paciente.telefono= value; }
        }

        public string Diagnostico
        {
            get { return _paciente.diagnostico; }
            set { _paciente.diagnostico = value; }
        }

        public string ZonaLesion
        {
            get { return _paciente.zonaLesion; }
            set { _paciente.zonaLesion = value; }
        }

        public DateTimeOffset FechaLesion
        {
            get { return _paciente.fechaLesion; }
            set { _paciente.fechaLesion = value; }
        }

        public async Task<bool> CoincidePaciente()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            var result = await connection.QueryAsync<Paciente>("Select apellidos FROM Pacientes WHERE nombre = ?", new object[] { Nombre });
            foreach (var item in result)
            {
                if (Apellidos.Equals(item.apellidos)) return true;
            }
            return false;
        }

        public async Task<bool> DoesDbExist(string DatabaseName)
        {
            bool dbexist = true;
            try
            {
                StorageFile storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(DatabaseName);

            }
            catch
            {
                dbexist = false;
            }
            return dbexist;
        }

        public async void CreateDatabase()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            await connection.CreateTableAsync<Paciente>();
            await connection.CreateTableAsync<Ejercicio>();
        }

        public async Task<bool> ObtenerPacienteInfo(string data){
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            var result = await connection.QueryAsync<Paciente>("Select * FROM Pacientes WHERE nombre = ?", new object[] { data });
            if (result.Count() == 0) return false;
            foreach (var item in result)
            {
                this.Id = item.id;
                this.Nombre = item.nombre;
                this.Apellidos = item.apellidos;
                this.Direccion = item.direccion;
                this.Localidad = item.localidad;
                this.Provincia = item.provincia;
                this.CodPostal = item.codigoPostal;
                this.Telefono = item.telefono;
                this.Diagnostico = item.diagnostico;
                this.ZonaLesion = item.zonaLesion;
                this.FechaLesion = item.fechaLesion;
            }
            return true;
        }

        public async Task ObtenerPacientePorId(string data)
        {
            Debug.WriteLine(data);
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            var result = await connection.QueryAsync<Paciente>("Select * FROM Pacientes WHERE id = ?", new object[] { data });
            if (result.Count() == 0) Debug.WriteLine("no hay naaaaa");
            foreach (var item in result)
            {
                this.Id = item.id;
                this.Nombre = item.nombre;
                this.Apellidos = item.apellidos;
                this.Direccion = item.direccion;
                this.Localidad = item.localidad;
                this.Provincia = item.provincia;
                this.CodPostal = item.codigoPostal;
                this.Telefono = item.telefono;
                this.Diagnostico = item.diagnostico;
                this.ZonaLesion = item.zonaLesion;
                this.FechaLesion = item.fechaLesion;
            }

        }

        public async Task InsertarNuevoPaciente()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            await connection.InsertAsync(this._paciente);
        }

        public async Task UpdateDatosPaciente(int id)
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            var Patient = await connection.Table<Paciente>().Where(x => x.id == id).FirstOrDefaultAsync();

            if (Patient != null)
            {
                Patient.nombre = this.Nombre;
                Patient.apellidos = this.Apellidos;
                Patient.direccion = this.Direccion;
                Patient.telefono = this.Telefono;
                Patient.provincia = this.Provincia;
                Patient.localidad = this.Localidad;
                Patient.codigoPostal = this.CodPostal;
                Patient.diagnostico = this.Diagnostico;
                Patient.zonaLesion = this.ZonaLesion;
                Patient.fechaLesion = this.FechaLesion;

                await connection.UpdateAsync(Patient);
            }
        }

        public async Task UpdateMedicalInfoPaciente(String name)
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            var Patient = await connection.Table<Paciente>().Where(x => x.nombre.StartsWith(name)).FirstOrDefaultAsync();

            if (Patient != null)
            {
                Patient.diagnostico = this.Diagnostico;
                Patient.zonaLesion = this.ZonaLesion;
                Patient.fechaLesion = this.FechaLesion;

                await connection.UpdateAsync(Patient);
            }
        }

        public async Task<bool> DeleteInfoPaciente(int id)
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            var Patient = await connection.Table<Paciente>().Where(x => x.id == id).FirstOrDefaultAsync();

            if (Patient != null)
            {
                await connection.DeleteAsync(Patient);
this.Nombre = "";
this.Apellidos = "";
this.Direccion = "";
this.Telefono = "";
this.Provincia = "";
this.Localidad = "";
this.CodPostal = "";
this.Diagnostico = "";
this.ZonaLesion = "";
                return true;
            }
            return false;
        }
    }
}
