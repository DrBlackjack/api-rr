﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APIFilR.Model;
using Microsoft.Extensions.Configuration;
using APIFilR.Context;
using APIFilR.Helpers;
using Microsoft.AspNetCore.Authentication;
using System;
using Microsoft.AspNetCore.Http;
using System.Data.Entity;

namespace APIFilR
{
    [ApiController]
    [Route("[controller]")]
    public class RessourcesController : Controller
    {
        public IConfiguration _configure;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RessourcesController(IConfiguration configure, IHttpContextAccessor httpContextAccessor)
        {
            _configure = configure;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public ActionResult<bool> PostTodoItem()
        {
            using MainContext ctx = new MainContext();
            var util = new UTILISATEUR() { id_type_compte = 1, mail = "tt", mdp = "tt", nom = "tt", prenom = "tt", verifie = 1 };
            ctx.Add(util);
            //await ctx.SaveChangesAsync();
            return Ok("1");
        }

        [HttpGet("GetTypeRelationRessource")]
        public ActionResult<type_relation_ressource> GetTypeRelationRessource()
        {
            using MainContext ctx = new MainContext();
            return Ok(ctx.Type_Relation_Ressource.ToList());
        }

        [HttpGet("GetCategoriesRessources")]
        public ActionResult<CATEGORIES_RESSOURCES> GetCategoriesRessources()
        {
            using MainContext ctx = new MainContext();
            return Ok(ctx.Categories_ressources.ToList());
        }

        [HttpGet("GetTypeRessources")]
        public ActionResult<TYPE_RESSOURCES> GetTypeRessources()
        {
            using MainContext ctx = new MainContext();
            return Ok(ctx.Type_Ressources.ToList());
        }

        [HttpGet("GetStatutRessource")]
        public ActionResult<type_relation_ressource> GetStatutRessource()
        {
            using MainContext ctx = new MainContext();
            return Ok(ctx.Statut_ressource.ToList());
        }

        [HttpGet("GetAllRessources")]
        public ActionResult<RESSOURCES> GetRessources()
        {
            using MainContext ctx = new MainContext();
            return Ok(ctx.Ressources.ToList());
        }

        [HttpPost("PostRessource/{email}")]
        public async Task<ActionResult<RESSOURCES>> PostRessource([FromBody] CreateRessource res, string email)
        {
            // Check jwt token
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Substring(7);            
            if (!TokenHelper.ValidateToken(token)) return BadRequest("Mauvais login");

            // On post la resource
            using MainContext ctx = new MainContext();
            var utilisateur = ctx.utilisateur.First(t => t.mail == email);

            var ressource = new RESSOURCES()
            {
                titre_ressource = res.titreRessource,
                description_ressource = res.descriptionRessource,
                date_creation_ressource = DateTime.Now,
                chemin_document = "",
                id_categories = res.idCategories,
                id_statut = res.idStatut,
                id_type = res.idType,
                id_utilisateur = utilisateur.id_utilisateur,
            };

            ctx.Ressources.Add(ressource);
            await ctx.SaveChangesAsync();
            return Ok(ctx.Ressources.ToList());
        }

        [HttpGet("GetCommentaire/{idRessource}")]
        public ActionResult<CommentaireDisplay[]> GetCommentaire(int idRessource)
        {
            using MainContext ctx = new MainContext();
            using MainContext ctx2 = new MainContext();
            var commentaires1 = ctx.Commentaires.Include(b => b.Utilisateur).ToList();
            var commentaires = ctx.Commentaires
            .Include(c => c.Utilisateur)
            .Where(com => com.id_ressource == idRessource).ToList()
                .Select(com =>
                {
                    return new CommentaireDisplay
                    {
                        idUtilisateur = com.id_utilisateur,
                        utilisateur = com.Utilisateur.prenom + " " + com.Utilisateur.nom,
                        commentaire = com.commentaire
                    };
                });
            return Ok(commentaires);
        }

        [HttpPost("PostCommentaire/{email}")]
        public async Task<ActionResult> PostRessource([FromBody] COMMENTAIRES com, string email)
        {
            // Check jwt token
            string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Substring(7);            
            if (!TokenHelper.ValidateToken(token)) return BadRequest("Mauvais login");

            // On post la resource
            using MainContext ctx = new MainContext();

            com.id_utilisateur = ctx.utilisateur.First(t => t.mail == email).id_utilisateur;

            ctx.Commentaires.Add(com);
            await ctx.SaveChangesAsync();
            return Ok();
        }
    }
}
