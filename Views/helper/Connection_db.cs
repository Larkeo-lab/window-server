using System;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace My_program.Views.helper
{
    public class Connection_db
    {
        public MySqlConnection connectdb;

        // ສ້າງເມັດທອດ constructor ຂອງຄລາດ
        public Connection_db()
        {
            string connection_string = DatabaseConfig.GetConnectionString();
            connectdb = new MySqlConnection(connection_string);
        }

        // ທົດສອບການເຊື່ອມຕໍ່ຖານຂໍ້ມູນ
        public async Task<bool> TestConnection()
        {
            try
            {
                // ເປີດການເຊື່ອມຕໍ່
                connectdb.Open();

                Console.WriteLine("✅ ເຊື່ອມຕໍ່ຖານຂໍ້ມູນສຳເລັດ!");
                Console.WriteLine($"📊 MySQL Version: {connectdb.ServerVersion}");
                Console.WriteLine($"🗄️  Database: {connectdb.Database}");
                Console.WriteLine($"🖥️  Server: {connectdb.DataSource}");

                // ປິດການເຊື່ອມຕໍ່
                connectdb.Close();
                Console.WriteLine("🔒 ປິດການເຊື່ອມຕໍ່ແລ້ວ\n");

                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"❌ MySQL Error: {ex.Message}");
                Console.WriteLine($"Error Number: {ex.Number}");

                string errorMessage = ex.Number switch
                {
                    0 => "ບໍ່ສາມາດເຊື່ອມຕໍ່ກັບ Server ໄດ້ - ກະລຸນາກວດສອບວ່າ MySQL ກຳລັງເປີດຢູ່",
                    1042 => "ບໍ່ສາມາດເຊື່ອມຕໍ່ກັບ Server ໄດ້",
                    1045 => "Username ຫຼື Password ບໍ່ຖືກຕ້ອງ",
                    1049 => "ບໍ່ພົບຖານຂໍ້ມູນທີ່ລະບຸ",
                    _ => $"ເກີດຂໍ້ຜິດພາດ: {ex.Message}"
                };

                Console.WriteLine($"💡 {errorMessage}\n");
                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}\n");
                throw new Exception($"ເກີດຂໍ້ຜິດພາດ: {ex.Message}");
            }
        }
    }
}
