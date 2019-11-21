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
        //
        public static SqlConnection con =
            new SqlConnection(ConfigurationManager.ConnectionStrings["MsSqlConnection"].ToString());

        // GET: /Base/DRZ_PAYER_SPECIFICATION
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
        #region"sp 사용하는 방법"
        // POST: /Base/                                                       
        [HttpPost]
        public JArray Index( )
        {

            //System.Diagnostics.Debug.WriteLine(test.log);
            //System.Diagnostics.Debug.WriteLine(test.sendinfo);

            #region"쿼리문"
            /*
            //쿼리문 전송
            string quary = "select * from my_test_table;";
            SqlCommand cmd2 = new SqlCommand(quary, con);
            cmd2.CommandType = CommandType.Text;
            con.Open();
            my_test_table dataList = cmd2.ExecuteReader();
            */
            #endregion


            //저장 프로시저 사용
            SqlCommand cmd = new SqlCommand("DAAPR_FAMILY_SELECT", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID_SABUN", 19821001);

            con.Open();
            SqlDataReader readData = cmd.ExecuteReader();

            var jsondata = ToJson(readData);
            System.Diagnostics.Debug.Write(jsondata);
            var json3Array = JArray.Parse(jsondata);

            /*
            var jsonArray2 = new JArray();
            while (rd.Read()) {
                var json2 = new JObject();
                System.Diagnostics.Debug.WriteLine(rd.FieldCount);
                for(int i=0;i<rd.FieldCount;i++){
                    System.Diagnostics.Debug.Write(rd.GetString(i)+"");
                }
                System.Diagnostics.Debug.WriteLine("");
                json2.Add("CD_FAMILY", rd.GetString(1));
                jsonArray2.Add(json2);
            }
             */

            //cmd.CommandText = string.Format("");

            //cmd.CommandType = CommandType.Text;

            //cmd.ExecuteNonQuery();

            //SqlDataReader 종료
            //connection 종료

            cmd.Dispose();
            readData.Close();
            con.Close();


            //Test 클래스로 데이터 받고 사용하기
            //json Array
            var jsonArray = new JArray();
            var json = new JObject();
            //json.Add("log", test.log);
            //json.Add("sendinfo", test.sendinfo);
            jsonArray.Add(json);

            return json3Array;
        }
        #endregion

        //테이블을 json으로 변경
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


    }
}
