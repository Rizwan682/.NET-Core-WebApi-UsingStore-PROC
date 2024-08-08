using ADOCRUD8_0.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using ADOCRUD8_0.Helpers;
namespace ADOCRUD8_0.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly SqlHelper _sqlHelper;

        public ProductController(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

     

        [Route("GetAllProduct")]
        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {


            try
            {
                SqlConnection con = _sqlHelper.GetConnection();
                if (con == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error while creating SQL connection.");
                }

                List<ProductModel> productModels = new List<ProductModel>();
                DataTable dt = new DataTable();
                using (con)
                {
                    SqlCommand cmd = new SqlCommand("SP_GetAllProducts", con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }



                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ProductModel productModel = new ProductModel();
                    productModel.ProductID = Convert.ToInt32(dt.Rows[i]["ID"]);
                    productModel.ProductName = dt.Rows[i]["PName"].ToString(); // Assuming ProductName is a string
                    productModel.ProductPrice = Convert.ToDecimal(dt.Rows[i]["PPrice"]);
                    productModel.EntryDate = Convert.ToDateTime(dt.Rows[i]["PEntryDate"]); // Assuming ProductEntryDate is a DateTime
                    productModel.ProductDescription = dt.Rows[i]["PDescription"].ToString();
                    productModel.ProdtModel = dt.Rows[i]["PModel"].ToString();
                    productModels.Add(productModel);
                }

                return Ok(productModels);
            }
            catch (Exception ex)
            {
                string exceptionMessage = ex.Message;
                return BadRequest(exceptionMessage);
            }
        }


        //[Route("PostProduct")]
        //[HttpPost]
        //public async Task<IActionResult> PostProduct(ProductModel obj)
        //{
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefualtConnection")))
        //        {
        //            string query = "INSERT INTO TBLProduct (PName, PPrice, PDescription, PModel, PEntryDate) VALUES (@PName, @PPrice, @PDescription, @PModel, GETDATE())";
        //            using (SqlCommand cmd = new SqlCommand(query, con))
        //            {
        //                cmd.Parameters.AddWithValue("@PName", obj.ProductName);
        //                cmd.Parameters.AddWithValue("@PPrice", obj.ProductPrice);
        //                cmd.Parameters.AddWithValue("@PDescription", obj.ProductDescription);
        //                cmd.Parameters.AddWithValue("@PModel", obj.ProdtModel);

        //                con.Open();
        //                await cmd.ExecuteNonQueryAsync();
        //                con.Close();
        //            }
        //        }
        //        return Ok(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}



        [Route("PostProduct")]
        [HttpPost]
        public async Task<IActionResult> PostProduct(ProductModel obj)
        {
            try
            {
                using (SqlConnection con = _sqlHelper.GetConnection())
                {
                    if (con == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Error while creating SQL connection.");
                    }

                    using (SqlCommand cmd = new SqlCommand("SP_InsertProduct", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PName", obj.ProductName);
                        cmd.Parameters.AddWithValue("@PPrice", obj.ProductPrice);
                        cmd.Parameters.AddWithValue("@PEntryDate", obj.EntryDate);  // I was missed this  <<--- Cmd parameter and i face the error message
                        cmd.Parameters.AddWithValue("@PDescription", obj.ProductDescription);
                        cmd.Parameters.AddWithValue("@PModel", obj.ProdtModel);

                        con.Open();
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        con.Close();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Product added successfully", product = obj });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, "Product not added successfully");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //[Route("UpdateProduct")]
        //[HttpPut]
        //public async Task<IActionResult> UpdateProduct(ProductModel obj)
        //{

        //    try
        //    {

        //        SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefualtConnection"));
        //        SqlCommand cmd = new SqlCommand("Update TBLProduct SET Pname='" + obj.ProductName + "',Pprice='" + obj.ProductPrice + "',PDescription ='" + obj.ProductDescription + "',PModel='" + obj.ProductPrice + "'Where Id='" + obj.ProductID + "' ", con);
        //        con.Open();
        //        cmd.ExecuteNonQuery();
        //        con.Close();
        //        return Ok(obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;

        //    }
        //}



        [Route("UpdateProduct")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductModel obj)
        {
            try
            {
                SqlConnection con = _sqlHelper.GetConnection();
                if (con == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error while creating SQL connection.");
                }

                using (con)
                {

                    string query = "SP_UpdateProduct";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure; // <<--- i missed this cmd  Command Type 
                        cmd.Parameters.AddWithValue("@PName", obj.ProductName);
                        cmd.Parameters.AddWithValue("@PPrice", obj.ProductPrice);
                        cmd.Parameters.AddWithValue("@PEntryDate", obj.EntryDate); /// <<--- i missed this paramater
                        cmd.Parameters.AddWithValue("@PDescription", obj.ProductDescription);
                        cmd.Parameters.AddWithValue("@PModel", obj.ProdtModel);
                        cmd.Parameters.AddWithValue("@ID", obj.ProductID);

                        con.Open();
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        con.Close();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Product Edited successfully", product = obj });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, "Product not Edited successfully");
                        }
                    }
                }
           
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        //    [Route("DeleteProduct/{id}")]
        //    [HttpDelete]
        //    public async Task<IActionResult> DeleteProduct(int id)
        //    {
        //        try
        //        {
        //            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefualtConnection"));
        //            SqlCommand cmd = new SqlCommand("DELETE FROM TBLProduct WHERE Id=@Id", con);
        //            cmd.Parameters.AddWithValue("@Id", id);
        //            con.Open();
        //            await cmd.ExecuteNonQueryAsync();
        //            con.Close();
        //            return Ok(new { message = "Product deleted successfully" });
        //        }
        //        catch (Exception ex)
        //        {
        //            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //        }
        //    }
        //}



        [Route("DeleteProduct/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                SqlConnection con = _sqlHelper.GetConnection();
                if (con == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error while creating SQL connection.");
                }

                using (con)
                {
                    string query = "SP_DeleteProduct";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ID", id);

                        con.Open();
                        await cmd.ExecuteNonQueryAsync();
                        con.Close();
                    }
                }
                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }

}

