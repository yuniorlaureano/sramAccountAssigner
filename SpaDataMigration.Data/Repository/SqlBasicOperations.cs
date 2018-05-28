using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using SramAccountAssigner.Entity;

namespace SramAccountAssigner.Data.Repository
{
    public class SqlBasicOperations
    {
        private ConnectionManager connectionManager; //Nameja las cadenas de coneccion
        private SqlConnection sqlConnection;
        private Schema schema; //Representa la cadena de coneccion
        private Country country;

        public SqlBasicOperations()
        {
            connectionManager = new ConnectionManager();
        }

        /// <summary>
        /// Abre la coneccion
        /// </summary>
        /// <param name="schema">Represanta la cadena de coneccion en el Web.config</param>
        /// <param name="country">codigo iso en tres digitos del mapais</param>
        public void OpenConnection(Schema schema, Country country)
        {
            try
            {
                if (this.sqlConnection == null)
                {
                    sqlConnection = new SqlConnection(connectionManager.SetConnectionString(schema, country));
                    this.schema = schema;
                    this.country = country;
                }

                if (this.schema != schema || this.country != country) // cambia el esquema en caso de que cambie 
                {
                    if (this.sqlConnection.State == ConnectionState.Open)
                    {
                        this.CloseConnection();
                    }

                    sqlConnection = new SqlConnection(connectionManager.SetConnectionString(schema, country));
                    this.schema = schema;
                    this.country = country;
                }

                if (this.sqlConnection.State == System.Data.ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }
            }
            catch (SqlException excep)
            {
                throw excep;
            }
            catch (ArgumentNullException excep)
            {
                throw excep;
            }
        }

        /// <summary>
        /// Cierra la conneccion
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                if (this.sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    this.sqlConnection.Close();
                }
            }
            catch (SqlException excep)
            {
                throw excep;
            }
        }

        /// <summary>
        /// Ejecuta una sentencia en la base de datos, y retorna un resultset con un dataset.
        /// </summary>
        /// <param name="query">sentencia sql</param>
        /// <param name="sqlParameters">arreglo con los parametros sql</param>
        /// <param name="commandType">Si es procedure y simple sentencia</param>
        /// <param name="schema">Cadena de conneccion</param>
        /// <param name="country">codigo iso en tres digitos del mapais</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataAdapter(string query, SqlParameter[] sqlParameters, CommandType commandType, Schema schema, Country country)
        {
            DataSet resultset = new DataSet();

            try
            {
                this.OpenConnection(schema, country);
                SqlCommand sqlCommand = new SqlCommand(query, this.sqlConnection);
                sqlCommand.CommandType = commandType;

                if (sqlParameters != null)
                {
                    sqlCommand.Parameters.AddRange(sqlParameters);
                }

                SqlDataAdapter oracledataAdapter = new SqlDataAdapter(sqlCommand);
                oracledataAdapter.Fill(resultset);
            }
            catch (SqlException excep)
            {
                throw excep;
            }
            finally
            {
                this.CloseConnection();
            }

            return resultset;
        }

        /// <summary>
        /// Ejecuta una sentencia en la base de datos, y retorna un booleano. utilizado para cuando no es necesario tener un resultset desde la base de datos.
        /// </summary>
        /// <param name="query">sentencia sql</param>
        /// <param name="sqlParameters">arreglo con los parametros sql</param>
        /// <param name="commandType">Si es procedure y simple sentencia</param>
        /// <param name="schema">Cadena de conneccion</param>
        /// <param name="country">codigo iso en tres digitos del mapais</param>
        /// <returns>bool</returns>
        public int ExecuteNonQuery(string query, SqlParameter[] sqlParameters, CommandType commandType, Schema schema, Country country)
        {
            int resultset = 0;

            try
            {
                this.OpenConnection(schema, country);
                SqlCommand sqlCommand = new SqlCommand(query, this.sqlConnection);
                sqlCommand.CommandType = commandType;

                if (sqlParameters != null)
                {
                    sqlCommand.Parameters.AddRange(sqlParameters);
                }

                resultset = sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException excep)
            {
                throw excep;
            }

            return resultset;
        }

        /// <summary>
        /// Ejecuta una sentencia en la base de datos, y retorna un datareader. En caso de requerir mas performance.+
        /// </summary>
        /// <param name="query">sentencia sql</param>
        /// <param name="sqlParameters">arreglo con los parametros sql</param>
        /// <param name="commandType">Si es procedure y simple sentencia</param>
        /// <param name="schema">Cadena de conneccion</param>
        /// <param name="country">codigo iso en tres digitos del mapais</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader ExecuteDataReader(string query, SqlParameter[] sqlParameters, CommandType commandType, Schema schema, Country country)
        {
            SqlDataReader resultset = null;

            try
            {
                this.OpenConnection(schema, country);
                SqlCommand sqlCommand = new SqlCommand(query, this.sqlConnection);
                sqlCommand.CommandType = commandType;

                if (sqlParameters != null)
                {
                    sqlCommand.Parameters.AddRange(sqlParameters);
                }

                resultset = sqlCommand.ExecuteReader();
            }
            catch (SqlException excep)
            {
                throw excep;
            }

            return resultset;
        }
    }
}
