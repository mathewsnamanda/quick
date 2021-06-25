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
    class Class2 : Interface2
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
                            
                            int p = t.Memo.IndexOf("-");
                            string spp = t.Memo.Substring(p+1,t.Memo.Length-p-1).ToLower();
                              if (spp.ToString().Contains(","))
                            {
                                string home = spp.ToString();
                                if (home.Contains(", &"))
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
                                        testp.Add(new test
                                        {

                                            Subtotal = t.Subtotal,
                                            Memo = spp + "-" + splitcount[i].ToString()


                                        });
                                        total += t.Subtotal;


                                    }
                                }

                            }
                            else
                            {
                                if (spp.ToString().Contains("&"))
                                {
                                    string[] splitcount = spp.Split(new char[] { '&' });
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
                                                Memo = spp + "-" + splitcount[i].ToString()


                                            });
                                            total += t.Subtotal;


                                        }
                                    }

                                }
                                else
                                {
                                    spp = spp.ToString().Trim();

                                    spp = spp.ToString().Trim();
                                    if (spp.ToLower().ToString() == unitid)
                                    {
                                        testp.Add(new test
                                        {

                                            Subtotal = t.Subtotal,
                                            Memo = spp + "-" + spp.ToString()


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
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return total;

        }
    }
}
