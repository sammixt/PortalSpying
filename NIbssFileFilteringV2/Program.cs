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
            var items = GetKeysAndColumns();



            InputParams inputParams = new InputParams(args);
            //string fullPath = @"D:\RPAFiles\NIBSS\Files\2020-12-16\SESSION_4\NIP\NIP_050_inwards successful.csv";
            string fullPath = @inputParams.FullPath;
            //int lastIndex = fullPath.LastIndexOf(@"\") + 1;
            //string filename = fullPath.Substring(lastIndex, fullPath.Length - lastIndex);
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
                //Console.WriteLine(file.CellName);
                allowedCell.Add(file.CellName);
                keyValuePairs.Add(file.CellName, file.ColumnName);
                //Console.WriteLine(file.ColumnName);
                //DO WORK
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
            string HDRString = "Yes";
            bool output = false;

            try
            {
                DataTable table = new DataTable();
                var getKeysAndColumns = GetKeysAndColumns();
                //string Folder = @"\\Mac\Home\Desktop\Nip Settlement\16Dec2020";
                //string FileName = "NIP_050_outwards successful.csv";
                //Split_Path(CSV_Location, Folder, Filename);
                string Folder = @folder;
                string FileName = fileName;
                OleDbConnection cn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + Folder + ";Extended Properties=\"Text;HDR=" + HDRString + " IMEX=1;FMT=Delimited\"");
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

                //log.Info($"Removing Rows with empty Amount columns");
                //foreach (DataRow row in table.Select(String.Format("[{0}] is null ", "F6")))
                //{
                //    row.Delete();
                //}
                //table.AcceptChanges();

                //log.Info($"Removing unwanted rows");
                //foreach (DataRow row in table.Select(String.Format("[{0}] = 'CHANNEL' ", "F2")))
                //{
                //    row.Delete();
                //}
                //table.AcceptChanges();


                //string columnsToRemove = settlementType == "NIPINWARD"
                //    ? "columnsToRemoveInward".GetKeyValue()
                //    : "columnsToRemoveOutward".GetKeyValue();
                //string allowedColumns = "allowedColumns".GetKeyValue();

                //string[] cols = allowedColumns.Split(',');
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
                //var filteredTable = processFile.FilterByEBN(table);
                var filteredTable = FilterBy(table, filterColumnName, filterCondition);
                int filterTableCount = filteredTable.Rows.Count;
                //Add new column session and Settlement Type

                //DataColumn SettlementType = new DataColumn("SETTLEMENTTYPE", typeof(System.String));
                //SettlementType.DefaultValue = settlementType;
                //table.Columns.Add(SettlementType);
                //DataColumn Session = new DataColumn("SESSION", typeof(string));
                //Session.DefaultValue = session;
                //table.Columns.Add(Session);
                //table.AcceptChanges();

                string connectionString = ConfigurationManager.ConnectionStrings["DbCon"].ConnectionString;
                string databaseTable = "destinationTable".GetKeyValue();
                SqlDatabase database = new SqlDatabase(connectionString, databaseTable, log);
                output = database.InsertRecord(table);
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
                                where data.Field<string>(filterColumn).Contains(filterCondition)
                                select data;//.CopyToDataTable();
            var tt = filteredBy.Count();
            //BoData = null;
            //SettlementData = null;
            var Result = tt == 0 ? new DataTable() : filteredBy.CopyToDataTable();
            return Result;
        }
    }
}
