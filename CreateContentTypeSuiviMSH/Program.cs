using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
namespace CreateContentTypeSuiviMSH
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();


            p.run();

            Console.WriteLine("done");
            Console.ReadLine();
        }


        public static readonly SPContentTypeId myContentTypeId = new SPContentTypeId("0x010100FA0963FA69A646AA916D2E41284FC9D9");

        public List<Guid> fieldsId = new List<Guid>(){
              //Guid.Parse("{256ed3a8-f6de-47a8-a208-0592a30ba0cb}"),
              //Guid.Parse("{59c7ad74-4149-4052-bdc9-9fc83156edec}"),
              //Guid.Parse("{9ebe1c00-5cd6-4aea-9432-ee55a7210579}"),
              //Guid.Parse("{c1138fac-b944-41c8-b544-0d9de452be36}"),
              //Guid.Parse("{c8ae8b3e-f553-4347-8ca3-1b86f9f48e18}"),
              //Guid.Parse("{fec85a1b-dad8-4cda-9aa3-e07175abde47}"),
              //Guid.Parse("{dbeed181-1ba6-486a-96dc-98f4ec30448a}"),
              //Guid.Parse("{15d9ff01-9e69-4380-8262-c485c894c6f5}"),
              //Guid.Parse("{30d808bc-713d-437d-b9ef-11dbf9ed3377}"),
              //Guid.Parse("{84a20d0b-a81b-42e7-a5b4-a6d037f8bde0}"),
              //Guid.Parse("{14dacf5d-65f2-434a-8d9c-51a1f3984553}"),
              //Guid.Parse("{9b50bd69-8f44-4c48-b0b5-d91277e05396}"),
              //Guid.Parse("{f2c91e81-acb8-4fd9-8837-0aefeda281d1}"),
              //Guid.Parse("{a101396c-d03e-4da1-963b-f959440670c4}"),
              //Guid.Parse("{c143d446-cdab-42b4-a1bf-96f03a576f8e}"),
              //Guid.Parse("{7b0a14cd-c512-4e4c-853a-03c6915f1eaf}"),
              //Guid.Parse("{df12080f-c6d2-423f-83bf-f600d7e470af}"),
              //Guid.Parse("{353733c9-d549-4c5e-ba38-cf903c329737}"),
              //Guid.Parse("{d524a041-00a4-468b-a277-a9f46c818451}"),
              //Guid.Parse("{e2290a3b-81f2-494d-b1ef-ecaab1dfb2a3}"),
              //Guid.Parse("{fc5ac977-9704-4ac1-a2ab-6a4530c2b0e8}"),
              //Guid.Parse("{1e4bcf1e-fae8-4327-848e-43281ae52265}"),
              //Guid.Parse("{7b3734c6-5d83-47fc-8366-daad7792c79a}"),
              //Guid.Parse("{6e1bc14d-87fe-4112-8464-3d5a96dc0d20}"),
              //Guid.Parse("{47be179e-0208-4b7b-bba4-e0fc3ce1a041}"),
              //Guid.Parse("{046c3c79-8fbc-4714-aa12-440efaa4524f}"),
              //Guid.Parse("{0f275f76-edaf-48b3-8784-79af274e7789}"),
              //Guid.Parse("{86c1cf9d-0221-4532-8577-48651a074b4d}"),
              //Guid.Parse("{23e64e96-d446-4172-ade8-59e9a3e87d73}"),
              //Guid.Parse("{514c6e38-0943-490c-9e49-ad9a1510a6fa}"),
              //Guid.Parse("{87442ed4-8015-4f07-a2a2-ea14db5deaca}"),
              //Guid.Parse("{9aa3e394-75f2-46ed-8125-ef32a9c97344}"),
              Guid.Parse("{749DA0D1-4649-4C25-871B-05F0C07221FC}"),
              Guid.Parse("{352EB708-3FC0-4757-B11A-B345E2B11540}"),
              //Guid.Parse("{3DBE4EEE-885D-43DF-819E-CD6C875B1995}"),
              //Guid.Parse("{A5034D85-8EE0-4E1B-BC2E-075D87DCCC13}"),
              //Guid.Parse("{7D2BB96F-D7B8-4EAB-AEF8-096480F99E60}"),
              //Guid.Parse("{36B95E9C-931F-457F-8C9A-90051DBC44FA}")
        };

        private void run()
        {
            string siteurl = "http://rdits-sp13-dev/sites/rnvo";

            using (SPSite site = new SPSite(siteurl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPContentType myContentType = web.ContentTypes[myContentTypeId];
                    if (myContentType != null)
                    {
                        web.ContentTypes.Delete(myContentTypeId);
                        web.Update();
                    }
                    
                    myContentType = new SPContentType(myContentTypeId, web.ContentTypes, "My Content Type");
                        myContentType.Group = "EDF";
                        web.ContentTypes.Add(myContentType);

                    foreach (Guid Id in fieldsId)
                    {
                        SPField field = web.AvailableFields[Id];
                        SPFieldLink fieldLink = new SPFieldLink(field);

                        if (myContentType.FieldLinks[fieldLink.Id] == null)
                        {
                            myContentType.FieldLinks.Add(fieldLink);
                        }
                    }
                    
                    myContentType.Update(true);

                }
            }




        }


    }
}
