﻿namespace APIFilR.Model
{
	public class RESSOURCES
	{
		public int id_ressource { get; set; }
		public string statut_ressource { get; set; }
		public string titre_ressource { get; set; }
		public string description_ressource { get; set; }
		public System.DateTime date_creation_ressource { get; set; }
		public string chemin_document { get; set; }
		public int id_type { get; set; }
		public int id_categories { get; set; }
		public int id_statut { get; set; }
		public int id_utilisateur { get; set; }
		public int id_statut_affecter { get; set; }
	}
}