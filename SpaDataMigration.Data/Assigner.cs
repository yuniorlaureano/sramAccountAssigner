using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SramAccountAssigner.Data.Repository;
using SramAccountAssigner.Entity;
using System.Data.SqlClient;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace SramAccountAssigner.Data
{
    public enum ENVVAR
    {
        PRI = 21,
        DOM = 45
    }

    public class Assigner
    {
        private OracleBasicsOperations oraclebasicOperations;
        public Country Country { get; set; }
        public ENVVAR Envvar { get; set; }

        public Assigner(Country country, ENVVAR envvar)
        {
            this.oraclebasicOperations = new OracleBasicsOperations();
            this.Country = country;
            this.Envvar = envvar;
        }

        /*
         --72115 ANYELIS
         --73168 RAQUEL SENA
             */
        public DataTable AutoAssign(string userCode, string auditors)
        {
            DataTable resultset = null;

            OracleParameter[] prm = 
            {
                new OracleParameter { ParameterName = "in_usercode", OracleDbType = OracleDbType.Varchar2, Value = userCode },
                new OracleParameter { ParameterName = "IN_AUDITORS", OracleDbType = OracleDbType.Varchar2, Value = auditors },
                new OracleParameter { ParameterName = "resultset", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output}
            };
            
            try
            {
                resultset = this.oraclebasicOperations.ExecuteDataAdapter("SFA.spyn_assign_rdv", prm, CommandType.StoredProcedure, Entity.Schema.SFA, this.Country).Tables[0];
            }
            catch (SqlException except)
            {
                throw except;
            }
            catch (ArgumentNullException except)
            {
                throw except;
            }
            finally
            {
                this.oraclebasicOperations.CloseConnection();
            }

            return resultset;
        }
        
        /// <summary>
        /// Generar un archivo de excel a partir de un datatable
        /// </summary>
        /// <param name="table">Datatle</param>
        /// <param name="sheetName">Nombre de la hoja de excel</param>
        /// <param name="fileName">Nombre de archivo de excel</param>
        /// <param name="savingPath">Ruta donde se almacena el archivo</param>
        /// <returns></returns>
        public string WriteToExcel(DataTable table, string sheetName, string fileName, string savingPath)
        {
            string archivo = savingPath + fileName + ".xlsx";
            List<string> header = GetDataTableHeader(table.Columns);

            using (SpreadsheetDocument workbook = SpreadsheetDocument.Create(archivo, SpreadsheetDocumentType.Workbook))
            {
                OpenXmlWriter writer;

                workbook.AddWorkbookPart();
                WorksheetPart wsp = workbook.WorkbookPart.AddNewPart<WorksheetPart>();

                writer = OpenXmlWriter.Create(wsp);
                writer.WriteStartElement(new Worksheet());
                writer.WriteStartElement(new SheetData());

                WriteExcelHeader(header.ToArray(), writer);
                WriteExcelValues(table, header, writer);

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Close();

                writer = OpenXmlWriter.Create(workbook.WorkbookPart);
                writer.WriteStartElement(new Workbook());
                writer.WriteStartElement(new Sheets());

                writer.WriteElement(new Sheet()
                {
                    Name = sheetName,
                    SheetId = 1,
                    Id = workbook.WorkbookPart.GetIdOfPart(wsp)
                });

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Close();

                workbook.Close();
            }

            return archivo;
        }

        /// <summary>
        /// Escribe el las columnas de datatable, al header del excel 
        /// </summary>
        /// <param name="header">Lista con las columnas del datatable</param>
        /// <param name="writer">Objeto excel sobre el que se escribira</param>
        public void WriteExcelHeader(string[] header, OpenXmlWriter writer)
        {
            List<OpenXmlAttribute> row = new List<OpenXmlAttribute> { new OpenXmlAttribute("r", null, "1") };
            writer.WriteStartElement(new Row(), row);
            List<OpenXmlAttribute> cell = new List<OpenXmlAttribute> { new OpenXmlAttribute("t", null, "inlineStr") };
            foreach (string h in header)
            {
                writer.WriteStartElement(new Cell(), cell);
                writer.WriteElement(new InlineString(new Text(h)));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Escribe los rows de un datatbale a excel
        /// </summary>
        /// <param name="table">Datatle</param>
        /// <param name="header">Lista con las columnas del datatable</param>
        /// <param name="writer">Objeto excel sobre el que se escribira</param>
        public void WriteExcelValues(DataTable table, List<string> header, OpenXmlWriter writer)
        {
            List<OpenXmlAttribute> row;
            List<OpenXmlAttribute> cell = new List<OpenXmlAttribute> { new OpenXmlAttribute("t", null, "inlineStr") };
            //List<OpenXmlAttribute> intCell = new List<OpenXmlAttribute> { new OpenXmlAttribute("t", null, "n") };
            int count = (table.Rows.Count + 1);
            for (int i = 1; i < count; i++)
            {
                row = new List<OpenXmlAttribute> { new OpenXmlAttribute("r", null, (i + 1).ToString()) };
                writer.WriteStartElement(new Row(), row);

                foreach (string th in header)
                {
                    writer.WriteStartElement(new Cell(), cell);
                    writer.WriteElement(new InlineString(new Text(table.Rows[i - 1][th].ToString())));
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Extrae las columnas de un datatable, y lo escribe a una lista de string
        /// </summary>
        /// <param name="columns">Las columnas del datatable</param>
        /// <returns>List<string></returns>
        private List<string> GetDataTableHeader(DataColumnCollection columns)
        {
            List<string> header = new List<string>();
            foreach (DataColumn dtc in columns)
            {
                header.Add(dtc.ColumnName);
            }

            return header;
        }

        private string CleanCustormerName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return name.Replace("'","''");
            }

            return name;
        }

    }
}
