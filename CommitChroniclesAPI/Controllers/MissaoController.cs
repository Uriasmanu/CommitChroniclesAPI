using CommitChroniclesAPI.DTOs;
using CommitChroniclesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class MissaoController : ControllerBase
{
    private readonly MissaoService _missaoService;

    public MissaoController(MissaoService missaoService)
    {
        _missaoService = missaoService;
    }

    [HttpPost]
    public async Task<IActionResult> CriarMissao([FromBody] MissaoDTO missaoDTO)
    {
        try
        {
            var missaoCriada = await _missaoService.CriarMissaoAsync(missaoDTO);
            return CreatedAtAction(nameof(ObterMissao), new { titulo = missaoDTO.Titulo }, missaoCriada);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<MissaoDTO>>> ObterMissoes()
    {
        try
        {
            var missoes = await _missaoService.ObterMissoesAsync();
            return Ok(missoes);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{titulo}")]
    public async Task<ActionResult<MissaoDTO>> ObterMissao(string titulo)
    {
        try
        {
            var missao = await _missaoService.ObterMissaoPorIdAsync(titulo);
            return Ok(missao);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{titulo}")]
    public async Task<IActionResult> AtualizarMissao(string titulo, [FromBody] MissaoDTO missaoDTO)
    {
        try
        {
            var missaoAtualizada = await _missaoService.AtualizarMissaoAsync(titulo, missaoDTO);
            return Ok(missaoAtualizada);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{titulo}")]
    public async Task<IActionResult> DeletarMissao(string titulo)
    {
        try
        {
            await _missaoService.DeletarMissaoAsync(titulo);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPatch("{titulo}/concluir")]
    public async Task<IActionResult> AlterarStatusConclusao(string titulo)
    {
        try
        {
            var missaoAtualizada = await _missaoService.AlterarStatusConclusaoAsync(titulo);
            return Ok(missaoAtualizada);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
