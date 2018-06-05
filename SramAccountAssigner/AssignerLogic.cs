using System;
using System.Linq;
using System.Text;
using System.Data;

namespace SramAccountAssigner
{
    public class AssignerLogic
    {

        public int TotalAssigned { get; set; }
        public DataTable Assignent { get; set; }

        public AssignerLogic(DataTable assignent)
        {
            this.Assignent = assignent;
        }

        public StringBuilder GetHtml()
        {
            StringBuilder html = new StringBuilder();

            foreach (DataRow row in Assignent.Rows)
            {
                html.Append("<tr><td>"+row["USR_NOMBRE"] +"</td><td>"+ row["FECHA_VENTA"] + "</td><td>" + row["CANTIDAD"] + "</td></tr>");
            }

            return html;
        }
        
        public int GetTotalAssigned()
        {
            int count = Convert.ToInt32(this.Assignent.AsEnumerable().Sum(x => x.Field<decimal>("CANTIDAD")));
            return count;
        }

        public string GetSalesDate()
        {
            string date = string.Empty;

            string []dates = this.Assignent.AsEnumerable().Select(x => x.Field<string>("FECHA_VENTA")).Distinct().ToArray();
            
            foreach (string item in dates)
            {
                date += item +",";
            }

            if (dates.Length == 0 )
            {
                return "";
            }

            date = date.Substring(0, date.Length-1);

            return date;
        }
    }
}
