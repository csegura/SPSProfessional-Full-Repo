using System.Collections.Generic;
using System.Text;

namespace SPSProfessional.SharePoint.Admin.ListTools
{
    public class SPSWebPropertiesHelper
    {
        public static string Encode(Dictionary<string, Dictionary<string, string>> data)
        {
            var result = new StringBuilder();

            foreach (string field in data.Keys)
            {
                foreach (string mode in data[field].Keys)
                    result.Append(string.Format("{0}|{1}|{2}#", field, mode, data[field][mode]));
            }

            return result.ToString();
        }

        public static Dictionary<string, Dictionary<string, string>> Decode(string values)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            string[] fields = values.Split("#".ToCharArray());

            foreach (string field in fields)
            {
                if (!string.IsNullOrEmpty(field))
                {
                    string[] detailValues = field.Split("|".ToCharArray());
                    string fieldValue = detailValues[0];
                    string modeValue = detailValues[1];
                    string value = detailValues[2];

                    if (!result.ContainsKey(fieldValue))
                        result.Add(fieldValue, new Dictionary<string, string>());

                    result[fieldValue].Add(modeValue, value);
                }
            }

            return result;
        }
    }
}