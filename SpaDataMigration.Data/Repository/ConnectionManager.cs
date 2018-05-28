using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using SramAccountAssigner.Entity;

namespace SramAccountAssigner.Data
{
    

    
    public class ConnectionManager
    {
        public string ConnectionString { get; set; }
        public Schema Schema { get; set; }
        public Country Country { get; set; }
        
        /// <summary>
        /// Obtiene el connection string de acuerdo al pais.
        /// </summary>
        /// <param name="schema">La base de datas a la que se connectara</param>
        /// <param name="country">El pais donde se connectara</param>
        /// <returns>string</returns>
        public string SetConnectionString(Schema schema, Country country)
        {
            this.Country = country;
            this.Schema = schema;

            this.ConnectionString = ConfigurationManager.ConnectionStrings[this.Schema.ToString() + "_" + this.Country.ToString()].ToString();

            return ConnectionString;
        }
    }
}
