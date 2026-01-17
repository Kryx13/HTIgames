public interface IInteractable
{
    // Le texte qui s'affiche (ex: "Ramasser Pistolet")
    string InteractionPrompt { get; }

    // La fonction qui se lance quand le joueur appuie sur E
    void Interact(PlayerController player);
}