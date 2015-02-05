using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace agSeedMarshaller
{
    class Program
    {
        static string baseurl = "http://agseedselect.com/";


        static string selProdSelect = "select taguide_id, product_id, crop_id, comment, lastUpdate  from taguide_product p ;";
        static string selProdInsert = "BEGIN IF NOT EXISTS (select top 1 * from selectedproduct where id = @id) and EXISTS (select top 1 * from selectedcrop where id = @scid) BEGIN INSERT INTO selectedproduct (id, datecreated, datemodified, comments, guideid, selectedcropid, productid) VALUES (@id, @created, @modified, @comment, @gid, @scid, @pid) END END";
        static string selProdDelete = "BEGIN delete from selectedproduct END ";

        static string selCropSelect = "select tagc.taguide_id, c.crop_id, c.cropname, tagc.lastUpdate from taguide_crops tagc join crops c on tagc.crop_id = c.crop_id;";
        static string selCropInsert = "BEGIN IF NOT EXISTS (select top 1 * from selectedcrop where id = @id) AND EXISTS (select top 1 * from guide where id = @gid) BEGIN INSERT INTO selectedcrop (id, datecreated, datemodified, guideid, cropid, cropkey) VALUES (@id, @created, @modified, @gid, @cid, @cropkey) END END";
        static string selCropDelete = "BEGIN delete from selectedcrop END ";
        

        static string cropSelect = "select crop_id, cropname from crops;";
        static string cropInsert = "BEGIN IF NOT EXISTS (select top 1 * from crop where id = @id) BEGIN INSERT INTO crop (id, datecreated, datemodified, cropkey, name) VALUES (@id, @created, @modified, @cropkey, @name) END END";
        static string cropDelete = "BEGIN delete from crop END ";

        static string localCharsSelect = "SELECT c.characteristicstaguide_id, c.taguide_id, c.lastUpdate, c.value, c.newname, c.product_id, c.type, ct.havevalue, ct.name,  cr.cropname  FROM taguide_product_chart c join characteristics_ta_guide ct on c.characteristicstaguide_id = ct.characteristicstaguide_id join crops cr on cr.crop_id = ct.crop_id ;";
        static string localCharsInsert = "BEGIN IF NOT EXISTS (select top 1 * from LocalCharacteristic where id = @id) BEGIN INSERT INTO LocalCharacteristic (id, datecreated, datemodified, cropkey, name, hasvalue) VALUES (@id, @created, @modified, @crop, @name, @hasvalue) END END";
        static string localCharsDelete = "BEGIN delete from LocalCharacteristic END ";


//        static string localChartSelect = "SELECT c.taguide_id, c.product_id, c.lastUpdate, cr.cropname, count(*) as pages FROM taguide_product_chart c join characteristics_ta_guide ct on c.characteristicstaguide_id = ct.characteristicstaguide_id join crops cr on cr.crop_id = ct.crop_id group by c.taguide_id, c.product_id;";
        static string localChartSelect = "SELECT c.taguide_id, c.product_id, c.characteristicstaguide_id, c.type, c.ranking, c.value, ct.name, c.newname, c.lastUpdate, cr.cropname, cr.crop_id FROM taguide_product_chart c join characteristics_ta_guide ct on c.characteristicstaguide_id = ct.characteristicstaguide_id join crops cr on cr.crop_id = ct.crop_id;";
        static string localChartInsert = "BEGIN IF NOT EXISTS (select top 1 * from localchart where id = @id) and EXISTS (select top 1 id from guide where id = @gid) and EXISTS (select top 1 id from selectedcrop where id = @selcropid) BEGIN INSERT INTO localchart (id, datecreated, datemodified, cropkey, guideid, selectedcropid) VALUES (@id, @created, @modified, @crop, @gid, @selcropid) END END";
        static string localChartDelete = "BEGIN delete from localchart END ";
        static string localChartCellInsert = "BEGIN IF NOT EXISTS (select top 1 * from localchartcell where id = @id) and EXISTS (select top 1 * from selectedproduct where id = @pid) BEGIN INSERT INTO localchartcell (id, datecreated, datemodified, rating, value, chartid, selectedproductid, chartcharacteristicid) VALUES (@id, @created, @modified, @rating, @value, @chartid, @pid, @chartcharid) END END";
        static string localChartCellDelete = "BEGIN delete from localchartcell END ";
        static string localChartCharInsert = "BEGIN IF NOT EXISTS (select top 1 * from localchartcharacteristic where id = @id) and EXISTS (select top 1 * from localchart where id = @cid) BEGIN INSERT INTO localchartcharacteristic (id, datecreated, datemodified, altname, chartid, characteristicid) VALUES (@id, @created, @modified, @altname, @cid, @ccid) END END";
        static string localChartCharDelete = "BEGIN delete from localchartcharacteristic END ";

        static string natCharProdSelect = "SELECT c.characteristic_id as cid, pc.product_id as pid, pc.product_characteristic_id as id, pc.characteristic_value as pvalue, col.characteristic_value as cvalue, col.lastUpdate FROM characteristics c join product_characteristics pc on c.characteristic_id = pc.characteristic_id join characteristic_order_lookup col on col.characteristic_order_id = pc.characteristic_order_id where col.characteristic_id <> 2 and col.characteristic_id <> 21 and col.characteristic_id <> 144 ;";
        static string natCharProdInsert = "BEGIN IF NOT EXISTS (select top 1 * from nationalcharacteristicvalue where id = @id)  AND EXISTS (select top 1 * from product where id = @pid) BEGIN INSERT INTO nationalcharacteristicvalue (id, datecreated, datemodified, value, characteristicid, productid) VALUES (@id, @created, @modified, @value, @cid, @pid) END END";
        static string natCharProdDelete = "BEGIN delete from nationalcharacteristicvalue END ";

        static string natCharSelect = "SELECT c.characteristic_id as id, cr.cropname, c.characteristicname FROM rel_crops_characteristics rc join characteristics c on c.characteristic_id = rc.characteristic_id join crops cr on cr.crop_id = rc.crop_id ;";
        static string natCharInsert = "BEGIN IF NOT EXISTS (select top 1 * from nationalcharacteristic where id = @id) BEGIN INSERT INTO nationalcharacteristic (id, datecreated, datemodified, name, cropkey) VALUES (@id, @created, @modified, @name, @crop) END END";
        static string natCharDelete = "BEGIN delete from nationalcharacteristic END ";

        static string userSelect = "SELECT u.id,  u.lastupdate,  u.firstname,  u.lastname,  u.email,  u.password,  u.phone, (select role from db_role where id = u.id_role) as role, (select path from files where file_id = u.file_id) as profileImagePath FROM db_user u ";
        static string userInsert = "BEGIN IF NOT EXISTS (select top 1 * from useraccount where id = @id) BEGIN INSERT INTO useraccount (id, datecreated, datemodified, firstname, lastname, email, password, phone, profileimageid, role) VALUES (@id, @created, @modified, @firstname, @lastname, @email, @password, @phone, @profileimage, @role) END END";
        static string userDelete = "BEGIN delete from useraccount END ";

        static string productSelect = "SELECT product_id, productname, productname_without_trait, keystrengths, isnew, lastUpdate, (select cropname from agseed30.crops c where c.crop_id = p.crop_id) as cname FROM agseed30.products p;";
        static string productInsert = "BEGIN IF NOT EXISTS (select top 1 * from product where id = @id) BEGIN INSERT INTO product (id, cropkey, name, datecreated, datemodified, isnew, TraitId) VALUES (@id, @crop, @name, @created, @modified, @isNew, @traitid) END END";
        static string productDelete = "BEGIN delete from product END ";

        static string traitSelect = "select l.characteristic_order_id, pc.product_id, l.characteristic_value, l.lastUpdate, c.cropname from characteristic_order_lookup l join product_characteristics pc on pc.characteristic_order_id = l.characteristic_order_id join products p on p.product_id = pc.product_id join crops c on c.crop_id = p.crop_id where (l.characteristic_id = 2 or l.characteristic_id = 21 or l.characteristic_id = 144);";
        static string traitInsert = "BEGIN IF NOT EXISTS (select top 1 * from trait where id = @id) BEGIN INSERT INTO trait (id, code, datecreated, datemodified, cropkey) VALUES (@id, @code, @created, @modified, @cropkey) END END";
        static string traitDelete = "BEGIN delete from trait END ";

        static string ptSelect = "select * from characteristic_order_lookup l join product_characteristics pc on pc.characteristic_order_id = l.characteristic_order_id where  (l.characteristic_id = 2 or l.characteristic_id = 21 or l.characteristic_id = 144)";

        static string guideSelect = "SELECT taguide_id, taguide_name, user_id, note, lastUpdate, (select path from files where file_id = g.background_id) as bgpath FROM taguide g";
        static string guideDelete = "BEGIN delete from guide END ";
        static string guideInsert = "BEGIN IF NOT EXISTS (select top 1 * from guide where id = @id) BEGIN INSERT INTO guide (id, name, datecreated, datemodified, dealernotes, coverimage, ownerid, showcontactinfo) VALUES (@id, @name, @created, @modified, @notes, @coverimage, @ownerid, 1) END END";


        static string territorySelect = "SELECT dt.dsm_territory_id as id, dt.dsm_territory as dsmname, (select team_id from territory where dsm_territory_id = dt.dsm_territory_id limit 1) as tid,  (select teamname from teams where team_id = tid) as teamname FROM dsm_territory dt;";
//        static string territorySelect = "SELECT dt.dsm_territory_id as id, dt.dsm_territory as dsmname, t.lastUpdate, a.AZR as zone, z.zipcode, tm.teamname, tag.taguide_id as guide_id  FROM territory t join dsm_territory dt on dt.dsm_territory_id = t.dsm_territory_id join azr a on a.AZR_id = t.azr_id join zipcode z on z.zipcode_id = t.zipcode_id join teams tm  on tm.team_id = t.team_id join taguide_dsm tag on tag.dsm_territory_id = t.dsm_territory_id;";
        static string territoryZipcodeDelete = "BEGIN delete from territoryzipcode END";
        static string territoryGuideDelete = "BEGIN delete from territoryguide END";
        static string territoryDelete = "BEGIN delete from territory END";
        static string territoryInsert = "BEGIN IF NOT EXISTS (select top 1 * from territory where id = @id) BEGIN INSERT INTO territory (id, name, datecreated, datemodified, dsmname) VALUES (@id, @name, @created, @modified, @dsmname) END END";
        static string zipcodeInsert = "BEGIN IF NOT EXISTS (select top 1 * from zipcode where value = @zip) BEGIN insert into zipcode (value) values(@zip); END END";
        static string territoryZipcodeInsert = "BEGIN IF NOT EXISTS (select top 1 * from territoryzipcode where territoryid = @id and zipcode = @zip) BEGIN insert into territoryzipcode (territoryid, zipcode) values(@id, @zip); END END";
        static string territoryGuideInsert = "BEGIN IF NOT EXISTS (select top 1 * from territoryguide where territoryid = @id and guideid = @gid) and EXISTS (select top 1 * from guide where id = @gid) BEGIN insert into territoryguide (territoryid, guideid) values(@id, @gid); END END";

        
        
        //static string selterritorySelect = "SELECT td.taguide_id as gid, td.dsm_territory_id as did, td.lastUpdate, d.dsm_territory as dsm, cr.cropname FROM taguide_dsm td join dsm_territory d on d.dsm_territory_id = td.dsm_territory_id join taguide_crops tc on tc.taguide_id = td.taguide_id join crops cr on cr.crop_id = tc.crop_id ;";
        static string selterritorySelect = "SELECT td.taguide_id as gid, td.dsm_territory_id as did, td.lastUpdate, d.dsm_territory as dsm FROM taguide_dsm td join dsm_territory d on d.dsm_territory_id = td.dsm_territory_id";
        static string selterritoryDelete = "BEGIN delete from selectedterritory END";
        static string selterritoryInsert = "BEGIN IF NOT EXISTS (select top 1 * from selectedterritory where id = @id) and EXISTS (select top 1 * from guide where id = @gid) BEGIN INSERT INTO selectedterritory (id, datecreated, datemodified, guideid, territoryid) VALUES (@id, @created, @modified, @gid, @tid) END END";

        static string selDocumentSelect = "SELECT r.taguide_id as gid, r.related_information_id as rid, r.lastUpdate, ri.related_information_name as name from taguide_related r join related_information ri on ri.related_information_id = r.related_information_id ;";
        static string DocumentDelete = "BEGIN delete from document END";
        static string selDocumentDelete = "BEGIN delete from selecteddocument END";
        static string DocumentInsert = "BEGIN IF NOT EXISTS (select top 1 * from document where id = @id) BEGIN INSERT INTO document (id, datecreated, datemodified, name, doctypeid) VALUES (@id, @created, @modified, @name, '0') END END";
        static string selDocumentInsert = "BEGIN IF NOT EXISTS (select top 1 * from selecteddocument where id = @id) and EXISTS (select top 1 * from guide where id = @gid) BEGIN INSERT INTO selecteddocument (id, datecreated, datemodified, guideid, documentid) VALUES (@id, @created, @modified, @gid, @did) END END";

        static string fakeDocType = "BEGIN if not exists (select top 1 * from doctype where id = '0') BEGIN insert into doctype (id, name, datecreated, datemodified) values ('0', 'stubdoctype', '1/1/1980', '1/1/1980') END END";
        

        static void Main(string[] args)
        {
            SqlConnection msconn = new SqlConnection("SERVER=localhost;DATABASE=agSeed41_dev;USER=sa;PASSWORD=a");
            msconn.Open();

            MySqlConnection myconn = new MySqlConnection("server=192.168.1.107;port=3306;uid=root;password=;database=agseed30");
            myconn.Open();

            //ClearTables(msconn);


            //LoadCrops(msconn, myconn);
            //Console.WriteLine("crops done");

            //LoadCharacteristics(msconn, myconn);
            //Console.WriteLine("characters done");

            //LoadProduct(msconn, myconn);
            //Console.WriteLine("prods done");

            //LoadProductCharacteristics(msconn, myconn);
            //Console.WriteLine("prod char done");

            //LoadLocalCharacteristics(msconn, myconn);
            //Console.WriteLine("local chars done");

            //LoadUsers(msconn, myconn);
            //Console.WriteLine("users done");

            //LoadGuides(msconn, myconn);
            //Console.WriteLine("guides done");

            //LoadSelectedCrop(msconn, myconn);
            //Console.WriteLine("selcrop done");

            //LoadSelectedProduct(msconn, myconn);
            //Console.WriteLine("selprod done");

            LoadLocalChart(msconn, myconn);
            Console.WriteLine("local chart done");

            LoadTerritories(msconn, myconn);
            Console.WriteLine("terrs done");


            LoadSelectedTerritory(msconn, myconn);
            Console.WriteLine("selterrs done");

            LoadSelectedDocuments(msconn, myconn);

            myconn.Close();
            myconn.Dispose();
            msconn.Close();
            msconn.Dispose();
        }

        private static void ClearTables(SqlConnection msconn)
        {

            SqlCommand del = new SqlCommand(localChartCellDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(localChartCharDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(localChartDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(selProdDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(selCropDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(selDocumentDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(selterritoryDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(guideDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(userDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(DocumentDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(territoryDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(cropDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(localCharsDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(natCharProdDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(natCharDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(productDelete, msconn);
            del.ExecuteNonQuery();
            del = new SqlCommand(traitDelete, msconn);
            del.ExecuteNonQuery();

        }

        private static void LoadSelectedDocuments(SqlConnection msconn, MySqlConnection myconn)
        {
            MySqlCommand cmd = new MySqlCommand(selDocumentSelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<SelectedDocument> docs = new List<SelectedDocument>();

            SqlCommand fake = new SqlCommand(fakeDocType, msconn);
            fake.ExecuteNonQuery();


            while (rdr.Read())
            {
                // gid, rid, r.lastUpdate,  name
                SelectedDocument d = new SelectedDocument();
                d.gid = rdr.GetString("gid");
                d.rid = rdr.GetString("rid");
                d.last = rdr.GetDateTime("lastupdate");
                d.name = rdr.GetString("name");

                docs.Add(d);
            }

            rdr.Close();
            rdr.Dispose();

           

            foreach (SelectedDocument d in docs)
            {
                SqlCommand command = new SqlCommand(DocumentInsert, msconn);

                //@id, @created, @modified, @crop, @gid, @tid
                command.Parameters.Add(new SqlParameter("id", d.rid));
                command.Parameters.Add(new SqlParameter("created", d.last));
                command.Parameters.Add(new SqlParameter("modified", d.last));
                command.Parameters.Add(new SqlParameter("name", d.name));
                command.ExecuteNonQuery();

                command = new SqlCommand(selDocumentInsert, msconn);

                //@id, @created, @modified, @crop, @gid, @tid
                command.Parameters.Add(new SqlParameter("id", d.gid + "-" + d.rid));
                command.Parameters.Add(new SqlParameter("created", d.last));
                command.Parameters.Add(new SqlParameter("modified", d.last));
                command.Parameters.Add(new SqlParameter("gid", d.gid));
                command.Parameters.Add(new SqlParameter("did", d.rid));
                command.ExecuteNonQuery();
            }
        }

        private static void LoadSelectedTerritory(SqlConnection msconn,MySqlConnection myconn)
        {
            MySqlCommand cmd = new MySqlCommand(selterritorySelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<SelectedTerritory> terrs = new List<SelectedTerritory>();

            while (rdr.Read())
            {
                // td.taguide_id as gid, td.dsm_territory_id as did, td.lastUpdate, d.dsm_territory as dsm, cr.cropname 
                SelectedTerritory t = new SelectedTerritory();
                t.gid = rdr.GetString("gid");
                t.did = rdr.GetString("did");
                t.last = rdr.GetDateTime("lastupdate");
                t.dname = rdr.GetString("dsm");

                terrs.Add(t);
            }

            
            rdr.Close();
            rdr.Dispose();

            

            foreach (SelectedTerritory c in terrs)
            {
                SqlCommand command = new SqlCommand(selterritoryInsert, msconn);

                //@id, @created, @modified, @crop, @gid, @tid
                command.Parameters.Add(new SqlParameter("id", c.gid + "-" + c.did));
                command.Parameters.Add(new SqlParameter("created", c.last));
                command.Parameters.Add(new SqlParameter("modified", c.last));
//                command.Parameters.Add(new SqlParameter("crop", c.crop));
                command.Parameters.Add(new SqlParameter("gid", c.gid));
                command.Parameters.Add(new SqlParameter("tid", c.did));

                command.ExecuteNonQuery();
            }
        }

        private static void LoadSelectedCrop(SqlConnection msconn, MySqlConnection myconn)
        {
            MySqlCommand cmd = new MySqlCommand(selCropSelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<SelectedCroop> crops = new List<SelectedCroop>();

            while (rdr.Read())
            {
                SelectedCroop c = new SelectedCroop();
                c.last = rdr.GetDateTime("lastupdate");
                c.gid = rdr.GetString("taguide_id");
                c.cid = rdr.GetString("crop_id");
                c.id = c.cid + "-" + c.gid;
                c.cname = mapCrop( rdr.GetString("cropname"));

                crops.Add(c);
            }


            rdr.Close();
            rdr.Dispose();

            

            foreach (SelectedCroop c in crops)
            {
                SqlCommand command = new SqlCommand(selCropInsert, msconn);

                //@id, @created, @modified, @crop, @name, @gid
                command.Parameters.Add(new SqlParameter("id", c.id));
                command.Parameters.Add(new SqlParameter("created", c.last));
                command.Parameters.Add(new SqlParameter("modified", c.last));
                command.Parameters.Add(new SqlParameter("cropkey", c.cname));
                command.Parameters.Add(new SqlParameter("gid", c.gid));
                command.Parameters.Add(new SqlParameter("cid", c.cid));

                command.ExecuteNonQuery();
            }
        }

        private static void LoadSelectedProduct(SqlConnection msconn, MySqlConnection myconn)
        {
            MySqlCommand cmd = new MySqlCommand(selProdSelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<SelectedProduct> prods = new List<SelectedProduct>();

            while (rdr.Read())
            {
                SelectedProduct c = new SelectedProduct();
                c.gid = rdr.GetString("taguide_id");
                c.pid = rdr.GetString("product_id");
                c.cid = rdr.GetString("crop_id");
                c.id = c.gid + "-" + c.pid + "-" + c.cid;
                if (!rdr.IsDBNull(3))
                {
                    c.name = rdr.GetString("comment");
                }
                else
                {
                    c.name = string.Empty;
                }
                c.last = rdr.GetString("lastUpdate");

                prods.Add(c);
            }


            rdr.Close();
            rdr.Dispose();

            

            foreach (SelectedProduct c in prods)
            {
                SqlCommand command = new SqlCommand(selProdInsert, msconn);

                //@id, @created, @modified, @crop, @name, @gid
                command.Parameters.Add(new SqlParameter("id", c.id));
                command.Parameters.Add(new SqlParameter("created", c.last));
                command.Parameters.Add(new SqlParameter("modified", c.last));
                command.Parameters.Add(new SqlParameter("cropkey", c.name));
                command.Parameters.Add(new SqlParameter("comment", c.name));
                command.Parameters.Add(new SqlParameter("gid", c.gid));
                command.Parameters.Add(new SqlParameter("scid", c.cid + "-" + c.gid));
                command.Parameters.Add(new SqlParameter("pid", c.pid));

                command.ExecuteNonQuery();
            }
        }



        private static void LoadCrops(SqlConnection msconn, MySqlConnection myconn)
        {           
            MySqlCommand cmd = new MySqlCommand(cropSelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<Crop> crops = new List<Crop>();

            while (rdr.Read())
            {
                Crop c = new Crop();
                c.id = rdr.GetString("crop_id");
                c.name = mapCrop( rdr.GetString("cropname"));

                crops.Add(c);

            }

            
            rdr.Close();
            rdr.Dispose();

            

            foreach (Crop c in crops)
            {
                SqlCommand command = new SqlCommand(cropInsert, msconn);

                //@id, @created, @modified, @crop, @name, @gid
                command.Parameters.Add(new SqlParameter("id", c.id));
                command.Parameters.Add(new SqlParameter("created", DateTime.Now));
                command.Parameters.Add(new SqlParameter("modified", DateTime.Now));
                command.Parameters.Add(new SqlParameter("cropkey", c.name));
                command.Parameters.Add(new SqlParameter("name", c.name));

                command.ExecuteNonQuery();
            }
        }


        private static List<LocalCharacteristic> chars = new List<LocalCharacteristic>();
        private static void LoadLocalCharacteristics(SqlConnection msconn, MySqlConnection myconn)
        {
            MySqlCommand cmd = new MySqlCommand(localCharsSelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                LocalCharacteristic c = new LocalCharacteristic();
                c.gid = rdr.GetString("taguide_id");
              //  c.id = rdr.GetString("product_id") + "-" + c.gid + "-" + rdr.GetString("characteristicstaguide_id") + "-" + rdr.GetString("type");
                c.id = rdr.GetString("characteristicstaguide_id");
                try
                {
                    c.last = rdr.GetDateTime("lastUpdate");
                }
                catch
                {
                    c.last = DateTime.Now;
                }

                c.crop = mapCrop( rdr.GetString("cropname"));
                c.name = rdr.GetString("name");
                c.hasValue = rdr.GetBoolean("havevalue");
                if (!rdr.IsDBNull(3))
                {
                    c.value = rdr.GetString("value");
                }
                else
                {
                    c.value = string.Empty;
                }

                if (!rdr.IsDBNull(4))
                {
                    c.newName = rdr.GetString("newname");
                }
                else
                {
                    c.newName = string.Empty;
                }

                chars.Add(c);
            }


            rdr.Close();
            rdr.Dispose();

            

            foreach (LocalCharacteristic c in chars)
            {
                SqlCommand command = new SqlCommand(localCharsInsert, msconn);

                //@id, @created, @modified, @crop, @name, @gid
                command.Parameters.Add(new SqlParameter("id", c.id));
                command.Parameters.Add(new SqlParameter("created", c.last));
                command.Parameters.Add(new SqlParameter("modified", c.last));
                command.Parameters.Add(new SqlParameter("crop", c.crop));
                command.Parameters.Add(new SqlParameter("name", c.name));
                command.Parameters.Add(new SqlParameter("hasvalue", c.hasValue));
//                command.Parameters.Add(new SqlParameter("value", c.value));
//                command.Parameters.Add(new SqlParameter("newname", c.newName));
                // command.Parameters.Add(new SqlParameter("gid", c.gid));

                command.ExecuteNonQuery();
            }

        }

        private static void LoadLocalChart(SqlConnection msconn, MySqlConnection myconn)
        {
            MySqlCommand cmd = new MySqlCommand(localChartSelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<LocalChart> charts = new List<LocalChart>();
            while (rdr.Read())
            {
                LocalChart c = new LocalChart();
                string pid = rdr.GetString("product_id");
                string gid = rdr.GetString("taguide_id");

               //  c.id = gid + "-" + pid;

                c.pid = pid;
                c.gid = gid;
                c.cropid = rdr.GetString("crop_id");
                c.ccid = rdr.GetString("characteristicstaguide_id");
                c.crop = mapCrop( rdr.GetString("cropname"));
                c.id = c.pid + "-" + c.gid + "-" + c.ccid + "-" + rdr.GetString("type");
                c.chartid = c.gid + "-" + c.crop;
                if (!rdr.IsDBNull(7)) { 
                    c.newname = rdr.GetString("newname");
                }
                else
                {
                    c.newname = string.Empty;
                }

                if (!rdr.IsDBNull(5))
                {
                    c.rowrvalue = rdr.GetString("value");
                }
                else
                {
                    c.rowrvalue = string.Empty;
                }

                if (!rdr.IsDBNull(4))
                {
                    c.rowrank = rdr.GetInt16("ranking");
                }
                else
                {
                    c.rowrank = 0;
                }

                c.rowname = rdr.GetString("name");

               try{
                   c.last = rdr.GetDateTime("lastUpdate");
               }catch{
                   c.last = DateTime.Now;
               }            

                charts.Add(c);
            }


            rdr.Close();
            rdr.Dispose();

            

            foreach (LocalChart c in charts)
            {
                SqlCommand command = new SqlCommand(localChartInsert, msconn);
                //@id, @name, @created, @modified, @dsmname
                command.Parameters.Add(new SqlParameter("id", c.chartid));
                command.Parameters.Add(new SqlParameter("created", c.last));
                command.Parameters.Add(new SqlParameter("modified", c.last));
                command.Parameters.Add(new SqlParameter("crop", c.crop));
                command.Parameters.Add(new SqlParameter("selcropid", c.cropid + "-" + c.gid));
                command.Parameters.Add(new SqlParameter("gid", c.gid));
//                command.Parameters.Add(new SqlParameter("pid", c.pid));

                command.ExecuteNonQuery();

                command = new SqlCommand(localChartCharInsert, msconn);
                //@id, @name, @created, @modified, @dsmname
                command.Parameters.Add(new SqlParameter("id", c.chartid + "-" + c.ccid));
                command.Parameters.Add(new SqlParameter("created", c.last));
                command.Parameters.Add(new SqlParameter("modified", c.last));
                command.Parameters.Add(new SqlParameter("altname", c.newname));
                command.Parameters.Add(new SqlParameter("cid", c.chartid));
                command.Parameters.Add(new SqlParameter("ccid", c.ccid));

                command.ExecuteNonQuery();


                //@id, @created, @modified, @rating, @value, @chartid, @pid, @chartcharid
                command = new SqlCommand(localChartCellInsert, msconn);
                //@id, @name, @created, @modified, @dsmname
                command.Parameters.Add(new SqlParameter("id", c.id));
                command.Parameters.Add(new SqlParameter("created", c.last));
                command.Parameters.Add(new SqlParameter("modified", c.last));
                command.Parameters.Add(new SqlParameter("rating", c.rowrank));
                command.Parameters.Add(new SqlParameter("value", c.rowrvalue));
                command.Parameters.Add(new SqlParameter("chartid", c.chartid));
                command.Parameters.Add(new SqlParameter("pid", c.gid + "-" + c.pid + "-" + c.cropid));
                command.Parameters.Add(new SqlParameter("chartcharid", c.chartid + "-" + c.ccid));

                command.ExecuteNonQuery();


            }
        }

        private static void LoadProductCharacteristics(SqlConnection msconn, MySqlConnection myconn)
        {
            MySqlCommand cmd = new MySqlCommand(natCharProdSelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<NationalProductCharacteristic> natachars = new List<NationalProductCharacteristic>();
            while (rdr.Read())
            {
                NationalProductCharacteristic n = new NationalProductCharacteristic();
                n.id = rdr.GetString("id").ToString();
                if (!rdr.IsDBNull(3))
                {
                    n.value =  rdr.GetString("pvalue");
                }
                else
                {
                    n.value = rdr.GetString("cvalue");
                }

                n.cid = rdr.GetString("cid");
                n.pid = rdr.GetString("pid");

                natachars.Add(n);
            }


            rdr.Close();
            rdr.Dispose();

            

            foreach (NationalProductCharacteristic n in natachars)
            {
                SqlCommand command = new SqlCommand(natCharProdInsert, msconn);
                //@id, @name, @created, @modified, @dsmname
                command.Parameters.Add(new SqlParameter("id", n.id));
                command.Parameters.Add(new SqlParameter("created", DateTime.Now));
                command.Parameters.Add(new SqlParameter("modified", DateTime.Now));
                command.Parameters.Add(new SqlParameter("value", n.value));
                command.Parameters.Add(new SqlParameter("cid", n.cid));
                command.Parameters.Add(new SqlParameter("pid", n.pid));

                command.ExecuteNonQuery();
            }
        }

        private static void LoadCharacteristics(SqlConnection msconn, MySqlConnection myconn)
        {
            MySqlCommand cmd = new MySqlCommand(natCharSelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<NationalCharacteristic> natachars = new List<NationalCharacteristic>();
            while (rdr.Read())
            {
                NationalCharacteristic n = new NationalCharacteristic();
                n.id = rdr.GetString("id").ToString();
                n.crop = mapCrop( rdr.GetString("cropname"));
                n.name = rdr.GetString("characteristicname");

                natachars.Add(n);
            }

            
            rdr.Close();
            rdr.Dispose();

           

            foreach (NationalCharacteristic n in natachars)
            {
                SqlCommand command = new SqlCommand(natCharInsert, msconn);
                //@id, @name, @created, @modified, @dsmname
                command.Parameters.Add(new SqlParameter("id", n.id));
                command.Parameters.Add(new SqlParameter("created", DateTime.Now));
                command.Parameters.Add(new SqlParameter("modified", DateTime.Now));
                command.Parameters.Add(new SqlParameter("name", n.name));
                command.Parameters.Add(new SqlParameter("crop", n.crop));

                command.ExecuteNonQuery();
            }
        }

        private static void LoadTerritories(SqlConnection msconn, MySqlConnection myconn)
        {
            MySqlCommand cmd = new MySqlCommand(territorySelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<Territory> territories = new List<Territory>();
            // dt.dsm_territory_id as id, dt.dsm_territory as dsmname, t.lastUpdate, a.AZR as zone, z.zipcode, tm.teamname, tag.taguide_id as guide_id 
            while (rdr.Read())
            {
                if (!rdr.IsDBNull(2) && !rdr.IsDBNull(3))
                {
                    Territory t = new Territory();
                    t.id = rdr.GetInt32("id").ToString();
                    t.name = rdr.GetString("dsmname");
                    t.created = DateTime.Now; // bugbug no dates set in old db
                    t.modified = t.created;
                    // t.zone = rdr.GetString("zone");
                    //t.zipcode = rdr.GetString("zipcode"); 
                    t.teamName = rdr.GetString("teamname");
                    // t.guideId = rdr.GetInt16("guide_id").ToString();

                    territories.Add(t);
                }
            }

            
            rdr.Close();
            rdr.Dispose();

           

            foreach (Territory t in territories)
            {
                SqlCommand command = new SqlCommand(territoryInsert, msconn);
                //@id, @name, @created, @modified, @dsmname
                command.Parameters.Add(new SqlParameter("id", t.id));
                command.Parameters.Add(new SqlParameter("created", t.created));
                command.Parameters.Add(new SqlParameter("modified", t.modified));
                command.Parameters.Add(new SqlParameter("name", t.name));
                command.Parameters.Add(new SqlParameter("dsmname", t.teamName));
                command.ExecuteNonQuery();

              //  command = new SqlCommand(zipcodeInsert, msconn);
              //  command.Parameters.Add(new SqlParameter("zip", t.zipcode));
              //  command.ExecuteNonQuery();

                /*
                command = new SqlCommand(territoryZipcodeInsert, msconn);
                command.Parameters.Add(new SqlParameter("id", t.id));
                command.Parameters.Add(new SqlParameter("zip", t.zipcode));
                command.ExecuteNonQuery();

                command = new SqlCommand(territoryGuideInsert, msconn);
                command.Parameters.Add(new SqlParameter("id", t.id));
                command.Parameters.Add(new SqlParameter("gid", t.guideId));
                command.ExecuteNonQuery();
                */
            }
        }

        private static void LoadUsers(SqlConnection msconn, MySqlConnection myconn)
        {
            MySqlCommand cmd = new MySqlCommand(userSelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<User> users = new List<User>();
            while (rdr.Read())
            {
                // SELECT u.id,  u.lastupdate,  u.firstname,  u.lastname,  u.email,  u.password,  u.phone, (select role from db_role where id = u.id_role) as role, (select path from files where file_id = u.file_id) as profileImagePath FROM db_user u 
                User u = new User();
                try
                {
                    u.created = rdr.GetDateTime("lastupdate");
                }
                catch
                {
                    u.created = DateTime.Now;
                }

                u.modified = u.created;
                u.firstName = rdr.GetString("firstname");
                u.lastName = rdr.GetString("lastname");
                u.id = rdr.GetString("id");
                u.password = rdr.GetString("password");
                if (!rdr.IsDBNull(6))
                {
                    u.phone = rdr.GetString("phone");
                }

                if (!rdr.IsDBNull(8))
                {
                    u.profileImage = baseurl + rdr.GetString("profileImagePath");
                }
                else
                {
                    u.profileImage = string.Empty;
                }
                u.role = MapRole(rdr.GetString("role"));
                u.email = rdr.GetString("email");

                users.Add(u);
            }

            
            rdr.Close();
            rdr.Dispose();

            foreach (User u in users)
            {
                SqlCommand command = new SqlCommand(userInsert, msconn);

                command.Parameters.Add(new SqlParameter("id", u.id));
                command.Parameters.Add(new SqlParameter("created", u.created));
                command.Parameters.Add(new SqlParameter("modified", u.modified));
                command.Parameters.Add(new SqlParameter("firstname", u.firstName));
                command.Parameters.Add(new SqlParameter("lastname", u.lastName));
                command.Parameters.Add(new SqlParameter("email", u.email));
                command.Parameters.Add(new SqlParameter("password", u.password));
                command.Parameters.Add(new SqlParameter("phone", u.phone));
                command.Parameters.Add(new SqlParameter("profileimage", u.profileImage));
                command.Parameters.Add(new SqlParameter("role", u.role));

                command.ExecuteNonQuery();
            }
        }

        private static string MapRole(string p)
        {
            string retval = string.Empty;
            switch (p)
            {
                case "Superuser":
                    retval = "Super";
                    break;
                case "Legal":
                    retval = "Legal";
                    break;
                case "Technical Agronomist":
                    retval = "Agronomist";
                    break;
            }
            return retval;
        }

        private static void LoadGuides(SqlConnection msconn, MySqlConnection myconn)
        {
            MySqlCommand cmd = new MySqlCommand(guideSelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<Guide> guides = new List<Guide>();

            while(rdr.Read())
            {
                
                // SELECT taguide_id, taguide_name, user_id, note, lastUpdate, (select path from files where file_id = g.background_id) as bgpath FROM taguide g
                Guide g = new Guide();
                g.id = rdr.GetString("taguide_id");
                g.created = DateTime.Now;
                g.updated = rdr.GetDateTime("lastUpdate");
                if(g.updated  < DateTime.Now)
                {
                    g.created = g.updated;
                }
                g.name = rdr.GetString("taguide_name");
                g.notes = rdr.GetString("note");
                if (!rdr.IsDBNull(5))
                {
                    g.coverimage = baseurl + rdr.GetString("bgpath");
                }
                else
                {
                    g.coverimage = string.Empty;
                }
                g.userid = rdr.GetInt16("user_id").ToString();

                if (g.userid != "0")
                {
                    guides.Add(g);
                }
            }

            rdr.Close();
            rdr.Dispose();

            

            foreach(Guide g in guides)
            {
                //@id, @name, @created, @modified, @notes, @coverimage, @ownerid

                SqlCommand command = new SqlCommand(guideInsert, msconn);

                command.Parameters.Add(new SqlParameter("id", g.id));
                command.Parameters.Add(new SqlParameter("created", g.created));
                command.Parameters.Add(new SqlParameter("modified", g.updated));
                command.Parameters.Add(new SqlParameter("name", g.name));
                command.Parameters.Add(new SqlParameter("notes", g.notes));
                command.Parameters.Add(new SqlParameter("coverimage", g.coverimage));
                command.Parameters.Add(new SqlParameter("ownerid", g.userid));

                command.ExecuteNonQuery();
            }
        }

        private static void LoadProduct(SqlConnection msconn, MySqlConnection myconn)
        {

            MySqlCommand cmd = new MySqlCommand(productSelect, myconn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<Product> prods = new List<Product>();

            // read old product in 
            while (rdr.Read())
            {
                string pname = rdr.GetString("productname");
                string pid = rdr.GetString("product_id");
                string cname = mapCrop( rdr.GetString("cname") );
                bool isNew = rdr.GetBoolean("isNew");
                DateTime upd = DateTime.Now;
                string keystr = String.Empty;
                //string ptrait = String.Empty;

                if (!rdr.IsDBNull(3))
                {
                    keystr = rdr.GetString("keystrengths"); 
                }

          //      if (!rdr.IsDBNull(1))
          //      {
          //          ptrait = rdr.GetString(1);
          //      }

                try
                {
                    upd = rdr.GetDateTime("lastUpdate");
                }
                catch { } // blind catch for 0 timestamp exception
                
                prods.Add(new Product(pid, upd, cname, pname, keystr, isNew));
            }


            rdr.Close();
            rdr.Dispose();

            cmd = new MySqlCommand(traitSelect, myconn);
            rdr = cmd.ExecuteReader();
            List<Trait> traits = new List<Trait>();
            traits.Add(new Trait() { id = "1066", cropname = "Corn", code = "UNKN", created = DateTime.Now, modified = DateTime.Now });

            // read old traits in 
            while (rdr.Read())
            {
                Trait t = new Trait();
                t.id = rdr.GetInt16("characteristic_order_id").ToString();
                t.code = rdr.GetString("characteristic_value");
                t.cropname = mapCrop(rdr.GetString("cropname"));

                try
                {
                    t.modified = rdr.GetDateTime("lastUpdate");
                }catch{
                    t.modified = DateTime.Now;
                }

                t.created = t.modified;

                traits.Add(t);
            }

            rdr.Close();
            rdr.Dispose();

            cmd = new MySqlCommand(ptSelect, myconn);
            rdr = cmd.ExecuteReader();
            List<PT> pts = new List<PT>();

            // read old trait/products in 
            while (rdr.Read())
            {
                PT pt = new PT();
                pt.productId = rdr.GetString("product_id");
                pt.traitId = rdr.GetString("characteristic_order_id");
                pt.traitName = rdr.GetString("characteristic_value");
                pts.Add(pt);
            }
            
            rdr.Close();
            rdr.Dispose();

            

            // store trait
            foreach (Trait t in traits)
            {
                    SqlCommand command = new SqlCommand(traitInsert, msconn);

                    command.Parameters.Add(new SqlParameter("id", t.id));
                    command.Parameters.Add(new SqlParameter("code", t.code));
                    command.Parameters.Add(new SqlParameter("cropkey", t.cropname));
                    command.Parameters.Add(new SqlParameter("created", t.created));
                    command.Parameters.Add(new SqlParameter("modified", t.modified));
    
                    command.ExecuteNonQuery();

            }

            // push into products
            foreach (Product p in prods)
            {
                PT pt = pts.Find(x => x.productId == p.id);
                if (pt != null)
                {
                    p.tid = pt.traitId;
                }
                else
                {
                    p.tid = "1066"; // magic number for unkn
                }

                SqlCommand command = new SqlCommand(productInsert, msconn);

                command.Parameters.Add(new SqlParameter("id", p.id));
                command.Parameters.Add(new SqlParameter("crop", p.crop));
                command.Parameters.Add(new SqlParameter("name", p.name));
                command.Parameters.Add(new SqlParameter("created", DateTime.Now));
                command.Parameters.Add(new SqlParameter("modified", p.modDate));
                command.Parameters.Add(new SqlParameter("isnew", p.isNew));
                command.Parameters.Add(new SqlParameter("traitid", p.tid));

                command.ExecuteNonQuery();

            }
        }

        private static string mapCrop(string p)
        {
            string retval = string.Empty;
            switch (p)
            {
                case "soybeans":
                    retval = "Soybeans";
                    break;
                case "cotton":
                    retval = "Cotton";
                    break;
                case "corn":
                    retval = "Corn";
                    break;
                case "alfalfa":
                    retval = "Alfalfa";
                    break;
                case "sorghum":
                    retval = "Sorghum";
                    break;
                case "spring canola":
                    retval = "SpringCanola";
                    break;
                case "winter canola":
                    retval = "WinterCanola";
                    break;
                case "corn silage":
                    retval = "Silage";
                    break;

            }

            return retval;
        }
    }
}
