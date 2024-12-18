using CachingMemory.Aplicacao.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CachingMemory.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private const string UsuariosCacheKey = "usuarios_lista";

        public UsuariosController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }


        [HttpPost("Incluir")]
        public IActionResult AdicionarUsuario([FromBody] UsuarioDto usuario)
        {
            string cacheKey = $"usuario_{usuario.Codigo}";
            _memoryCache.Set(cacheKey, usuario, TimeSpan.FromMinutes(10));

            if (_memoryCache.TryGetValue(UsuariosCacheKey, out List<UsuarioDto> usuarios))
            {
                usuarios.Add(usuario);
            }
            else
            {
                usuarios = new List<UsuarioDto> { usuario };
            }

            _memoryCache.Set(UsuariosCacheKey, usuarios, TimeSpan.FromMinutes(10));
            return Ok(new { Message = "Usuário adicionado ao cache", Usuario = usuario });
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodosUsuario()
        {
            if (_memoryCache.TryGetValue(UsuariosCacheKey, out List<UsuarioDto> usuarios))
            {
                return Ok(usuarios);
            }
            return NotFound(new { Message = "Nenhum usuário encontrado no cache" });
        }

        [HttpGet("ObterPorCodigo")]
        public IActionResult ObterUsuario(int codigo)
        {
            string cacheKey = $"usuario_{codigo}";

            if (_memoryCache.TryGetValue(cacheKey, out UsuarioDto usuario))
            {
                return Ok(usuario);
            }

            return NotFound(new { Message = "Usuário não encontrado no cache" });
        }
    }

}
