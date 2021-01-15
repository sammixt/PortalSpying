using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIbssFileFilteringV2
{
    class Program
    {
        static Logger log = new Logger();
        static void Main(string[] args)
        {
            
            InputParams inputParams = new InputParams(args);
            string fullPath = @inputParams.FullPath;
            string filename = $"{inputParams.SheetName}$";
            string folder = fullPath;
            string resultPath = @"downloadResultPath".GetKeyValue();

            log.Info($@"Extracting {inputParams.FullPath}");
            bool output = ReadToDataTable(folder, filename, inputParams.SettlementType, inputParams.Session,inputParams.FilterColumn,inputParams.FilterCondition);
            WriteOutput(output, resultPath);
            output.ToString().Dump();
        }

        public static Tuple<string[],Dictionary<string,string>> GetKeysAndColumns()
        {
            List<string> allowedCell = new List<string>();
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (CellElement file in RequiredCell.GetCells())
            {
                allowedCell.Add(file.CellName);
                keyValuePairs.Add(file.CellName, file.ColumnName);
               
            }

            return new Tuple<string[], Dictionary<string, string>>(allowedCell.ToArray(), keyValuePairs);
        }

        public static bool ReadToDataTable(string folder, 
            string fileName, 
            string settlementType, 
            string session, 
            string filterColumn,
            string filterCondition)
        {
            string HDRString = "No";
            bool output = false;

            try
            {
                DataTable table = new DataTable();
                var getKeysAndColumns = GetKeysAndColumns();
                string Folder = @folder;
                string FileName = fileName;
                OleDbConnection cn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + Folder + ";Extended Properties=\"Excel 12.0;HDR=" + HDRString + "  IMEX=1\"");
                OleDbDataAdapter da = new OleDbDataAdapter();
                DataSet ds = new DataSet();
                OleDbCommand cd = new OleDbCommand("SELECT * FROM [" + FileName + "]", cn);

                cn.Open();
                da.SelectCommand = cd;
                ds.Clear();
                log.Info($"Reading Data into collection");
                da.Fill(ds, "CSV");
                table = ds.Tables[0];
                cn.Close();
              
                log.Info($"Removing unwanted columns");

                var toBeRemoved = table.Columns.Cast<DataColumn>()
                 .Where(c => !getKeysAndColumns.Item1.Contains(c.ColumnName))
                 .ToList();

                foreach (DataColumn col in toBeRemoved)
                {
                    table.Columns.Remove(col);
                }
                table.AcceptChanges();
                foreach(KeyValuePair<string, string> entry in getKeysAndColumns.Item2)
                {
                    table.Columns[entry.Key].ColumnName = entry.Value;
                }
                
                table.AcceptChanges();
                string filterColumnName = string.Empty;
                if (getKeysAndColumns.Item2.ContainsKey(filterColumn))
                {
                    filterColumnName = getKeysAndColumns.Item2[filterColumn];
                }else
                {
                    throw new Exception($"Invalid Filter Column Supplied::{filterColumn}",
                        new Exception($"Provide a valid column as specified in the config file"));
                }

                log.Info($"Filtering {filterColumnName} By {filterCondition}");
                
                var filteredTable = FilterBy2(table, filterColumnName, filterCondition);
                int filterTableCount = filteredTable.Rows.Count;
                if(filterTableCount > 1)
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["DbCon"].ConnectionString;
                    string databaseTable = "destinationTable".GetKeyValue();
                    SqlDatabase database = new SqlDatabase(connectionString, databaseTable, log);
                    output = database.InsertRecord(filteredTable);
                }else
                {
                    log.Info("Filtered condition returns empty collection");
                }

                
            }
            catch (Exception ex)
            {
                log.Info("An error occurred, check the error log for more details");
                log.Error(ex);
            }
            return output;


        }

        public static void WriteOutput(bool result, string downloadPath)
        {
            if (File.Exists(downloadPath))
            {
                "Found an existing file.......".Dump();
                "Deleting existing file from location.........".Dump();
                File.Delete(downloadPath);
                File.WriteAllText(downloadPath, result.ToString());
            }
            else
            {
                File.WriteAllText(downloadPath, result.ToString());
            }
        }

        public static DataTable FilterBy(DataTable dataTable, string filterColumn, string filterCondition)
        {
           
            var filteredBy = from data in dataTable.AsEnumerable()
                                where (data.Field<string>(filterColumn) != "NULL" &&
                                data.Field<string>(filterColumn).StartsWith(filterCondition))
                                select data;
            var tt = filteredBy.Count();
           
            var Result = tt == 0 ? new DataTable() : filteredBy.CopyToDataTable();
            return Result;
        }

        public static DataTable FilterBy2(DataTable dataTable, string filterColumn, string filterCondition)
        {
            DataTable outputTable = dataTable.Clone();
            var expression = $"[{filterColumn}] like '%{filterCondition}%'";

            foreach(DataRow row in dataTable.Select(expression))
            {
                var NewRow = outputTable.NewRow();
                foreach (DataColumn c in NewRow.Table.Columns)
                    NewRow[c.ColumnName] = row[c.ColumnName];

                outputTable.Rows.Add(NewRow);
            }
            
            return outputTable;
        }
    }
}
