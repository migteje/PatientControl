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
        // Reglas para rellenar los campos

        // We allow all Unicode letter characters as well as internal spaces and hypens, as long as these do not occur in sequences.
        private const string NAMES_REGEX_PATTERN = @"\A\p{L}+([\p{Zs}\-][\p{L}]+)*\z";

        // We allow all Unicode letter and numeric characters as well as internal spaces, as long as these do not occur in sequences.
        private const string ADDRESS_REGEX_PATTERN = @"\A[\p{L}\p{N}]+([\p{Zs}][\p{L}\p{N}]+)*\z";

        // We allow all Unicode umeric characters and hypens, as long as these do not occur in sequences.
        private const string NUMBERS_REGEX_PATTERN = @"\A\p{N}+([\p{N}\-][\p{N}]+)*\z";

        public int Id
        {
            get { return _paciente.id; }
            set { _paciente.id = value; }
        }

        [Required(ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(NAMES_REGEX_PATTERN, ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RegexErrorMessage")]
        public string Nombre
        {
            get { return _paciente.nombre; }
            set { _paciente.nombre = value; }
        }

        [Required(ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(NAMES_REGEX_PATTERN, ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RegexErrorMessage")]
        public string Apellidos
        {
            get { return _paciente.apellidos; }
            set { _paciente.apellidos = value; }
        }

        [Required(ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(ADDRESS_REGEX_PATTERN, ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RegexErrorMessage")]
        public string Direccion
        {
            get { return _paciente.direccion; }
            set { _paciente.direccion = value; }
        }

        [RegularExpression(ADDRESS_REGEX_PATTERN, ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RegexErrorMessage")]
        public string DireccionOpcional
        {
            get { return _paciente.direccionOpcional; }
            set { _paciente.direccionOpcional= value; }
        }

        [Required(ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(ADDRESS_REGEX_PATTERN, ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RegexErrorMessage")]
        public string Provincia
        {
            get { return _paciente.provincia; }
            set { _paciente.provincia = value; }
        }

        [Required(ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(ADDRESS_REGEX_PATTERN, ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RegexErrorMessage")]
        public string Localidad
        {
            get { return _paciente.localidad; }
            set { _paciente.localidad = value; }
        }

        [Required(ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(NUMBERS_REGEX_PATTERN, ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "ZipCodeRegexErrorMessage")]
        [StringLength(6, MinimumLength = 5, ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "ZipCodeLengthInvalidErrorMessage")]
        public string CodPostal
        {
            get { return _paciente.codigoPostal; }
            set { _paciente.codigoPostal = value; }
        }

        [Required(ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RequiredErrorMessage")]
        [RegularExpression(NUMBERS_REGEX_PATTERN, ErrorMessageResourceType = typeof(ErrorMessagesHelper), ErrorMessageResourceName = "RegexErrorMessage")]
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

        public string ArticValorar
        {
            get { return _paciente.articValorar; }
            set { _paciente.articValorar = value; }
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
                this.DireccionOpcional = item.direccionOpcional;
                this.Localidad = item.localidad;
                this.Provincia = item.provincia;
                this.CodPostal = item.codigoPostal;
                this.Telefono = item.telefono;
                this.Diagnostico = item.diagnostico;
                this.ZonaLesion = item.zonaLesion;
                this.FechaLesion = item.fechaLesion;
                this.ArticValorar = item.articValorar;
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
                this.DireccionOpcional = item.direccionOpcional;
                this.Localidad = item.localidad;
                this.Provincia = item.provincia;
                this.CodPostal = item.codigoPostal;
                this.Telefono = item.telefono;
                this.Diagnostico = item.diagnostico;
                this.ZonaLesion = item.zonaLesion;
                this.FechaLesion = item.fechaLesion;
                this.ArticValorar = item.articValorar;
            }

        }

        public async Task InsertarNuevoPaciente()
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection("Paciente.db");
            await connection.InsertAsync(this._paciente);
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
                Patient.articValorar = this.ArticValorar;

                await connection.UpdateAsync(Patient);
            }
        }
    }
}
