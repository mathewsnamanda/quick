using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quick.Models;
using System.Data.Odbc;

namespace quick.methods
{
    class Class1 : Interface1
    {
        OdbcConnection DbConnection = new OdbcConnection("DSN=QuickBooks Data QRemote");
       
        public decimal results(string unitid)
        {
            unitid = unitid.ToLower();
            decimal total = 0;
            List<test> testp = new List<test>();
            List<test> test = new List<test>();
            using (DbConnection)
            {
                try
                {
                    DbConnection.Open();
                    OdbcCommand DbCommand = DbConnection.CreateCommand();
                    DbCommand.CommandText = $"SELECT [Subtotal],[Memo] from  salesreceipt  where [Memo] like '%{unitid}%'";
                    OdbcDataReader DbReader = DbCommand.ExecuteReader();

                    var dataTable = new DataTable();
                    dataTable.Load(DbReader);
                    if (dataTable.Rows.Count > 0)
                    {
                        var serializedMyObjects = JsonConvert.SerializeObject(dataTable);
                        // Here you get the object
                        test = (List<test>)JsonConvert.DeserializeObject(serializedMyObjects, typeof(List<test>));
                    }
                    foreach (var t in test)
                    {
                        if (t.Memo.Contains("-"))
                        {
                            string[] split = t.Memo.Split(new char[] { '-' });
                            if (split[1].ToString().Contains(","))
                            {
                                string home = split[1].ToString();
                                if(home.Contains(", &"))
                                {
                                    home = home.Replace(", &", "&");
                                }
                                if (home.Contains(" & ,"))
                                {
                                    home = home.Replace(" & ,", "&");
                                }
                                if (home.Contains(",&"))
                                {
                                    home = home.Replace(",&", "&");
                                }
                                if (home.Contains("&,"))
                                {
                                    home = home.Replace("&,", "&");
                                }
                                home = home.Replace(",", "&");
                                
                              

                                string[] splitcount = home.Split(new char[] { '&' });
                                for (int i = 0; i < splitcount.Length; i++)
                                {
                                    splitcount[i] = splitcount[i].Trim();
                                    splitcount[i] = splitcount[i].Trim();

                                    if (splitcount[i].ToLower().ToString() == unitid)
                                    {
                                        t.Subtotal = t.Subtotal / splitcount.Length;
                                         total += t.Subtotal;
                                    }
                                }

                            }
                            else
                            {
                                if (split[1].ToString().Contains("&"))
                                {
                                    string[] splitcount = split[1].Split(new char[] { '&' });
                                    for (int i = 0; i < splitcount.Length; i++)
                                    {
                                        splitcount[i] = splitcount[i].Trim();
                                        splitcount[i] = splitcount[i].Trim();

                                        if (splitcount[i].ToLower().ToString() == unitid)
                                        {


                                            t.Subtotal = t.Subtotal / splitcount.Length;
                                            testp.Add(new test
                                            {

                                                Subtotal = t.Subtotal,
                                                Memo = split[0] + "-" + splitcount[i].ToString()


                                            });
                                           total += t.Subtotal;


                                        }
                                    }

                                }
                                else
                                {
                                    split[1] = split[1].ToString().Trim();

                                    split[1] = split[1].ToString().Trim();
                                    if (split[1].ToLower().ToString() == unitid)
                                    {
                                        testp.Add(new test
                                        {
                                            Subtotal = t.Subtotal
                                        });
                                        total += t.Subtotal;
                                    }
                                    else
                                    {

                                    }

                                }
                            }

                        }
                        else { }
                    }

                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return total;
        }
    }
}
