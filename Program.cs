using System.Data;
using System.Data.SqlClient; //Import Library System.Data.SQLClient agar dapat mengakses database SQL Server

namespace DatabaseConnectivity;

//Membuat class Program
//Pada class program terdapat 6 method void yaitu method Main, GetAllRegion, InsertRegion, GetById, UpdateRegion, DeleteRegion
class Program 
{
    //ConnectionString digunakan sbg parameter ketika membuat instance server object shg terkoneksi ke instance Microsoft SQL Server
    //Dengan menggunakan data source "DESKTOP-1O93DB4\\MSSQLSERVER01" dan database db_hr_dts
    static string ConnectionString = "Data Source=DESKTOP-1O93DB4\\MSSQLSERVER01;Database=db_hr_dts;Integrated Security=True;Connect Timeout=30;";

    //Membuat instance server object dengan nama connection
    static SqlConnection connection;
    static void Main(string[] args)
    {
        char select = 'Y';
        do
        {
            Console.Clear();
            Console.WriteLine("== Database Connectivity ==");
            Console.WriteLine("\t1. Get All Region");
            Console.WriteLine("\t2. Get By Id = 1");
            Console.WriteLine("\t3. Insert Region = Indonesia");
            Console.WriteLine("\t4. Update Region Id = 5, Name = Malaysia");
            Console.WriteLine("\t5. Delete Region Id =5");
            Console.WriteLine();
            Console.Write("Input : ");
            //Menyimpan input user ke variable input
            int input = int.Parse(Console.ReadLine());
            //Kondisi dari input user
            if (input == 1)
            {
                GetAllRegion();                 //Menampilkan semua data region
                Console.Write("Back to Home? Y/N : ");
                select = char.Parse(Console.ReadLine());
            }
            else if (input == 2)
            {
                GetById(1); //Menampilkan data region berdasarkan id yaitu 1
                Console.Write("Back to Home? Y/N : ");
                select = char.Parse(Console.ReadLine());
            }
            else if (input == 3)
            {
                InsertRegion("Indonesia");    //Menambah data region yaitu Indonesia
                Console.Write("Back to Home? Y/N : ");
                select = char.Parse(Console.ReadLine());
            }
            else if (input == 4)
            {
                UpdateRegion(5, "Malaysia");  //Melakukan update, yaitu update data region dengan id=5, menjadi Malaysia
                Console.Write("Back to Home? Y/N : ");
                select = char.Parse(Console.ReadLine());
            }
            else if (input == 5)
            {
                DeleteRegion(5);              //Menghapus data region, dengan id = 5
                Console.Write("Back to Home? Y/N : ");
                select = char.Parse(Console.ReadLine());
            }
            else
            {
                Console.WriteLine("Input yang anda masukkan salah");

                Console.Write("Back to Home? Y/N : ");
                select = char.Parse(Console.ReadLine());
            }
        } while (select == 'Y' || select == 'y');
    }

    // GETALL : REGION (Command)
    public static void GetAllRegion()
    {
        //Membuat instance server object yang menampung data ConnectionString
        connection = new SqlConnection(ConnectionString);

        //Membuat instance untuk command
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        //Membuat SQL Command untuk menjalankan perintah DML Select yg dijalankan di server database untuk menampilkan semua data tabel region
        command.CommandText = "SELECT * FROM tbl_region"; 

        //Membuka koneksi
        connection.Open();

        //Menggunakan data reader untuk mengambil data
        using SqlDataReader reader = command.ExecuteReader();
        //Kondisi jika reader memiliki data maka akan menampilkan data kolom ke-0 dan kolom ke-1
        if (reader.HasRows)
        {
            while (reader.Read()) //Mendapatkan baris dari hasil kueri shg dapat mengakses setiap kolom dari baris yg dikembalikan
            {
                Console.WriteLine("Id: " + reader[0]); 
                Console.WriteLine("Name: " + reader[1]);
                Console.WriteLine("====================");
            }
        }
        //Jika tidak ada, maka akan menampilkan informasi "Data not found"
        else
        {
            Console.WriteLine("Data not found!");
        }
        //Menutup reader
        reader.Close();
        //Menutup koneksi
        connection.Close();
    }

    // GETBYID : REGION (Command)
    public static void GetById(int id)
    {
        connection = new SqlConnection(ConnectionString);

        //Membuat instance untuk command
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        //Membuat Command dengan paramater Id untuk menjalankan perintah DML Select yg dijalankan di server database untuk menampilkan data pada tabel region berdasarkan id
        command.CommandText = "SELECT * FROM tbl_region Where id = @id";

        //Membuka koneksi
        connection.Open();

        //Membuat parameter
        SqlParameter pId = new SqlParameter();
        pId.ParameterName = "@id";      
        pId.Value = id;
        pId.SqlDbType = SqlDbType.VarChar;

        //Menambahkan data parameter
        command.Parameters.Add(pId);

        //Menggunakan data reader untuk mengambil data
        using SqlDataReader reader = command.ExecuteReader();
        //Kondisi jika reader memiliki data maka akan menampilkan data kolom ke-0 dan kolom ke-1
        if (reader.HasRows)
        {
            while (reader.Read()) //Mendapatkan baris dari hasil kueri shg dapat mengakses setiap kolom dari baris yg dikembalikan
            {
                Console.WriteLine("Id: " + reader[0]);
                Console.WriteLine("Name: " + reader[1]);
                Console.WriteLine("====================");
            }
        }
        //Jika tidak ada, maka akan menampilkan informasi "Data not found"
        else
        {
            Console.WriteLine("Data not found!");
        }
        //Menutup reader
        reader.Close();
        //Menutup koneksi
        connection.Close();
    }

    // INSERT : REGION (Transaction)
    public static void InsertRegion(string name)
    {
        connection = new SqlConnection(ConnectionString);

        //Membuka koneksi
        connection.Open();

        //Membuat transaction, agar jika terjadi kegagalan dapat melakukan rollback
        SqlTransaction transaction = connection.BeginTransaction();
        try
        {
            //Membuat instance untuk command
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            
            //Membuat Command dengan paramater name untuk menjalankan perintah DML Insert yg dijalankan di server database untuk menambahkan data nama pada tabel region
            command.CommandText = "INSERT INTO tbl_region (name) VALUES (@name)"; 
            
            //
            command.Transaction = transaction;

            //Membuat parameter dengan nama pName
            SqlParameter pName = new SqlParameter();
            pName.ParameterName = "@name"; 
            pName.Value = name;
            pName.SqlDbType = SqlDbType.VarChar;

            //Menambahkan parameter ke command
            command.Parameters.Add(pName);

            //Menjalankan command
            int result = command.ExecuteNonQuery(); //hasil yang dikembalikan adalah int
            transaction.Commit(); //Menyimpan transaksi ke database

            //Kondisi, jika result yang dihasilkan command.ExecuteNonQuery berhasil atau lebih dari 0 maka akan menampilkan informasi "Data berhasil ditambahkan"
            if (result > 0)
            {
                Console.WriteLine("Data berhasil ditambahkan!");
            }
            //Jika perintah command.ExecuteNonQuery tidak berhasil atau kurang dari 0 maka akan menampilkan informasi data gagal ditambahkan
            else
            {
                Console.WriteLine("Data gagal ditambahkan!");
            }

            //Menutup koneksi
            connection.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            try
            {
                transaction.Rollback(); //Jika transaksi gagal, maka akan dijalankan perintah rollback untuk mengembalikan database ke bentuk awal
            }
            catch (Exception rollback)
            {
                Console.WriteLine(rollback.Message);
            }
        }
    }

    // UPDATE : REGION (Transaction)
    public static void UpdateRegion(int id, string name)
    {
        connection = new SqlConnection(ConnectionString);

        //Membuka koneksi
        connection.Open();

        //Membuat transaction, agar jika terjadi kegagalan dapat melakukan rollback
        SqlTransaction transaction = connection.BeginTransaction();
        try
        {
            //Membuat instance untuk command
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            //Membuat Command dengan paramater Id untuk menjalankan perintah DML Update yg dijalankan di server database untuk mengubah data pada tabel region berdasarkan id
            command.CommandText = "UPDATE tbl_region SET name = @name WHERE id = @id ";          
            command.Transaction = transaction;

            //Membuat parameter id
            SqlParameter pId = new SqlParameter();
            pId.ParameterName = "@id";
            pId.Value = id;
            pId.SqlDbType = SqlDbType.VarChar;

            //Membuat parameter name
            SqlParameter pName = new SqlParameter();
            pName.ParameterName = "@name";
            pName.Value = name;
            pName.SqlDbType = SqlDbType.VarChar;

            //Menambahkan parameter ke command
            command.Parameters.Add(pName);
            command.Parameters.Add(pId);

            //Menjalankan command
            int result = command.ExecuteNonQuery(); //hasil yang dikembalikan adalah int
            transaction.Commit(); //Menyimpan transaksi ke database

            //Kondisi, jika result yang dihasilkan command.ExecuteNonQuery berhasil atau lebih dari 0 maka akan menampilkan informasi "Data berhasil ditambahkan"
            if (result > 0)
            {
                Console.WriteLine("Data berhasil diubah!");
            }
            //Jika perintah command.ExecuteNonQuery tidak berhasil atau kurang dari 0 maka akan menampilkan informasi data gagal ditambahkan
            else
            {
                Console.WriteLine("Data gagal diubah!"); 
            }

            //Menutup koneksi
            connection.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            try
            {
                transaction.Rollback(); //Jika transaksi gagal, maka akan dijalankan perintah rollback untuk mengembalikan database ke bentuk awal
            }
            catch (Exception rollback)
            {
                Console.WriteLine(rollback.Message);
            }
        }
    }

    // DELETE : REGION (Transaction)
    public static void DeleteRegion(int id)
    {
        connection = new SqlConnection(ConnectionString);

        //Membuka koneksi
        connection.Open();

        //Membuat transaction, agar jika terjadi kegagalan dapat melakukan rollback
        SqlTransaction transaction = connection.BeginTransaction();
        try
        {
            //Membuat instance untuk command
            SqlCommand command = new SqlCommand();
            command.Connection = connection;

            //Membuat Command dengan paramater Id untuk menjalankan perintah DML Delete yg dijalankan di server database untuk menghapus data pada tabel region berdasarkan id
            command.CommandText = "DELETE FROM tbl_region WHERE id = @id ";
            command.Transaction = transaction;

            //Membuat parameter id
            SqlParameter pId = new SqlParameter();
            pId.ParameterName = "@id";
            pId.Value = id;
            pId.SqlDbType = SqlDbType.VarChar;

            //Menambahkan parameter ke command
            command.Parameters.Add(pId);

            //Menjalankan command
            int result = command.ExecuteNonQuery(); //hasil yang dikembalikan adalah int
            transaction.Commit(); //Menyimpan transaksi ke database

            //Kondisi, jika result yang dihasilkan command.ExecuteNonQuery berhasil atau lebih dari 0 maka akan menampilkan informasi "Data berhasil ditambahkan"
            if (result > 0)
            {
                Console.WriteLine("Data berhasil dihapus!");
            }
            //Jika perintah command.ExecuteNonQuery tidak berhasil atau kurang dari 0 maka akan menampilkan informasi data gagal ditambahkan
            else
            {
                Console.WriteLine("Data gagal dihapus!");
            }

            //Menutup koneksi
            connection.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            try
            {
                transaction.Rollback(); //Jika transaksi gagal, maka akan dijalankan perintah rollback untuk mengembalikan database ke bentuk awal
            }
            catch (Exception rollback)
            {
                Console.WriteLine(rollback.Message);
            }
        }
    }
}
