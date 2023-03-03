using System.Net;
using DotnetApi.Data;
using DotnetApi.Dtos;
using DotnetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetPosts")]
        public IEnumerable<Post> GetPosts()
        {
            return _dapper.LoadData<Post>("SELECT * FROM TutorialAppSchema.Posts");
        }
        [HttpGet("GetPost/{postId}")]
        public Post GetPost(int postId)
        {
            string getPostByIdSql = $@"SELECT * FROM TutorialAppSchema.Posts
            WHERE PostId = ${postId}
            AND UserId = {GetUserIdFromToken()}";
            return _dapper.LoadSingleData<Post>(getPostByIdSql);
        }
        [HttpGet("GetPostByUserId/{userId}")]
        public IEnumerable<Post> GetPostByUserId(int userId)
        {
            string getPostByUserIdSql = $"SELECT * FROM TutorialAppSchema.Posts WHERE UserId = {userId}";
            return _dapper.LoadData<Post>(getPostByUserIdSql);
        }
        [HttpGet("GetMyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string getMyPostsSql = $@"SELECT * FROM TutorialAppSchema.Posts WHERE UserId = '{GetUserIdFromToken()}'";
            return _dapper.LoadData<Post>(getMyPostsSql);
        }
        [HttpPost("AddPost")]
        public IActionResult AddPost(PostToAddDto post)
        {
            string InserPostSql = $@"
                INSERT INTO TutorialAppSchema.Posts
                (
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated]
                ) VALUES (
                    '{GetUserIdFromToken()}',
                    '{post.PostTitle}',
                    '{post.PostContent}',
                    '{DateTime.Now}',
                    '{DateTime.Now}'
                )
            ";

            if (_dapper.ExecuteSql(InserPostSql))
            {
                return Ok();
            }

            return BadRequest("Failed to Add Post");
        }
        [HttpPut("EditPost")]
        public IActionResult EditPost(PostToEditDto post)
        {
            string UpdatePostSql = $@"
                UPDATE TutorialAppSchema.Posts
                SET
                    [PostTitle] = '{post.PostTitle}',
                    [PostContent] = '{post.PostContent}',
                    [PostUpdated] = '{DateTime.Now}'
                WHERE UserId = '{GetUserIdFromToken()}'
            ";

            if (_dapper.ExecuteSql(UpdatePostSql))
            {
                return Ok();
            }

            return BadRequest("Failed to Edit Post");
        }
        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string deletePostSql = $@"
            DELETE FROM TutorialAppSchema.Posts
            WHERE PostId = '{postId.ToString()}'
            AND UserId = '{GetUserIdFromToken()}'";

            if (_dapper.ExecuteSql(deletePostSql))
            {
                return Ok();
            }

            return BadRequest("Failed to delete post");
        }


        private string? GetUserIdFromToken()
        {
            return this.User.FindFirst("UserId")?.Value;
        }

    }
}