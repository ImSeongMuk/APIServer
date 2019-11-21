using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MvcApplication.Controllers
{
    public class DRController : Controller
    {
        //
        // GET: /DR/
        public ActionResult Index()
        {
            return View();
        }

        public static SqlConnection con =
            new SqlConnection(ConfigurationManager.ConnectionStrings["MsSqlConnection"].ToString());

        // GET: /DR/DRZ_PAYER_SPECIFICATION
        [HttpGet]
        public JArray DRZ_PAYER_SPECIFICATION(string data)
        {
            //SELECT * FROM ECONNET.DRZFN_PAYER_SPECIFICATION_HOMEPAGE('1146AC','A','01','2401','17','003','','','')

            System.Diagnostics.Debug.WriteLine(data);
            var json = JObject.Parse(data);
            System.Diagnostics.Debug.WriteLine(json.SelectToken("@CD_ACNTUNIT"));

            String quary = String.Format("SELECT * from ECONNET.DRZFN_PAYER_SPECIFICATION_HOMEPAGE('{0}','A','01','2401','17','003','','','');",
                json.SelectToken("@CD_ACNTUNIT"));
            SqlCommand cmd = new SqlCommand(quary, con);
            cmd.CommandType = CommandType.Text;

            try
            {
                con.Open();

                var dataList = cmd.ExecuteReader();
                var jsonString = ToJson(dataList);
                var jsonArray = JArray.Parse(jsonString);

                dataList.Close();
                con.Close();
                return jsonArray;
            }
            catch (Exception ex)
            {
                JObject jsonO = new JObject();
                jsonO.Add("result", ex.ToString());
                var jsonArray = new JArray();
                jsonArray.Add(jsonO);
                return jsonArray;
            }

        }


        #region"DBSPR_IPJUASRECEIPT_AP"
        // POST: /Base/                                                       
        [HttpGet]
        public JArray DBSPR_IPJUASRECEIPT_AP(string data)
        {
            var json = JObject.Parse(data);
            System.Diagnostics.Debug.WriteLine(json.SelectToken("@CD_ACNTUNIT"));

            //저장 프로시저 사용
            SqlCommand cmd = new SqlCommand("DBSPR_IPJUASRECEIPT_API", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CD_SITE", json.SelectToken("@ID_SABUN"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@DS_DONG", json.SelectToken("@DS_DONG"));
            cmd.Parameters.AddWithValue("@DS_CUNG", json.SelectToken("@DS_CUNG"));
            cmd.Parameters.AddWithValue("@DS_HO", json.SelectToken("@DS_HO"));
            cmd.Parameters.AddWithValue("@NO_HADOCONT", json.SelectToken("@NO_HADOCONT"));
            cmd.Parameters.AddWithValue("@DT_JEOBSU", json.SelectToken("@DT_JEOBSU"));
            cmd.Parameters.AddWithValue("@RM_CUSTOMER", json.SelectToken("@RM_CUSTOMER"));
            cmd.Parameters.AddWithValue("@CD_UH", json.SelectToken("@CD_UH"));
            cmd.Parameters.AddWithValue("@CD_WI", json.SelectToken("@CD_WI"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));
            cmd.Parameters.AddWithValue("@CD_CTM", json.SelectToken("@CD_CTM"));

            try
            {
                con.Open();
                SqlDataReader readData = cmd.ExecuteReader();
                var jsondata = ToJson(readData);
                System.Diagnostics.Debug.Write(jsondata);
                var jsonArray = JArray.Parse(jsondata);

                cmd.Dispose();
                readData.Close();
                con.Close();

                return jsonArray;
            }
            catch (Exception ex)
            {
                JObject jsonO = new JObject();
                jsonO.Add("result", ex.ToString());
                var jsonArray = new JArray();
                jsonArray.Add(jsonO);
                return jsonArray;
            }
        }
        #endregion


        #region"json으로 변경"
        public string ToJson(SqlDataReader rdr)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.WriteStartArray();

                while (rdr.Read())
                {
                    jsonWriter.WriteStartObject();

                    int fields = rdr.FieldCount;

                    for (int i = 0; i < fields; i++)
                    {
                        jsonWriter.WritePropertyName(rdr.GetName(i));
                        jsonWriter.WriteValue(rdr[i]);
                    }

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndArray();

                return sw.ToString();
            }
        }
        #endregion

    }
}
