public enum CardColor
{
    Blue,
    Green,
    Red,
    Yellow,
}

public enum Phase 
{
    Draw,
    Main
}

// Dit is het interface wat er voor zorgt dat je daadwerkelijk meerdere type classes allemaal kan notifien
public interface ISubscriber {

    void Update(Action<Player> effect);
}

public interface IEnchantmentSubscriber {
    void Update(Action<Permanent> effect);
}


// De publisher is verantwoordelijk voor het bijhouden van de geinteresseerde entiteiten op het moment dat de kaart gespeeld wordt.
public interface IPublisher<T>{

    void Attach(ISubscriber subscriber);
    void Detach(ISubscriber subscriber);
    void Notify(T notification);
}
// Pub sub constructie, de landCard is verantwoordelijk voor het informeren van de entiteiten die moeten weten als deze kaart
// "Gespeeld" wordt.
// Nu doet performaction notifyen
public class LandCard : Card, IPublisher {
    public IList<ISubscriber> Subscribers = new List<ISubscriber>();
    public CardColor color;

    public Action<Player> Effect = (p) => {p.Mana+=1;}

    public void Attach(ISubscriber subscriber)
    {
        Subscribers.Add(subscriber);
    }

    public void Detach(ISubscriber subscriber)
    {
        Subscribers.Remove(subscriber);
    }

    public void Notify()
    {
        foreach(var subscriber in Subscribers)
        {
            subscriber.Update(Effect);
        }
    }

    public override void PerformAction()
    {
        Notify();
    }
}

// Onze abstract card
public abstract class Card
{
    public abstract void PerformAction();
}



// De player constructie, verantwoordelijk voor de kaarten in het deck etc
public class Player : ISubscriber
{
    public string Name;

    public int Mana = 0;

    public int Health = 20;

    public IList<Card> Deck = new List<Card>();

    public void Update(Action<Player> effect)
    {
        // Wij krijgen hier het effect van de kaart mee die we op de player moeten afspelen
        effect(this);
    }
}

// De gamelog is een voorbeeld van iets anders wat dan geinteresseerd zou kunnen zijn wanneer kaarten worden gespeeld
public class GameLog : ISubscriber
{
    public void Update(Action<Player> ef)
    {
        Console.WriteLine($"Card played");
    }
}

// De gamestate implementeerd het state pattern. Gebasseerd op de enum phase zorgt de gamestate ervoor dat het spel daadwerkelijk
// wordt gespeeld en de kaarten defineren alleen wat ze nou doen.
public class GameState
{
    public Player p1;
    public Player p2;
    public Phase phase;

    public void Tick()
    {
    }
}

// Factory om het creeren van een card en zijn subscribers op 1 plek te doen zodat je dat makkelijker per kaart kan doen.
public class CardFactory
{
    public static LandCard CreateLandCard(IList<ISubscriber> subscribers)
    {
        var card = new LandCard();
        card.Effect = (Player p) => { p.Mana += 1; };
        foreach(var s in subscribers)
        {
            card.Attach(s);
        }
        return card;
    }
    public static LandCard CreateLandCard(IList<ISubscriber> subscribers)
    {
        var card = new LandCard();
        card.Effect = (Player p) => { p.Mana += 1; };
        foreach(var s in subscribers)
        {
            card.Attach(s);
        }
        return card;
    }
    public static SpellCard CreateSpellCard(IList<ISubscriber> subscribers)
    {
        var card = new SpelLCard();
        card.Effect = (Player p) => { p.Mana--; };
        foreach(var s in subscribers)
        {
            card.Attach(s);
        }
        return card;
    }
}

// Onze entry point
public class Program
{
    public static void Main()
    {
        GameLog gameLog = new GameLog();
        Player p1 = new Player();
        p1.Name = "Jim";
        Player p2 = new Player();
        p1.Name = "Jim2";
        LandCard card = CardFactory.CreateLandCard(new List<ISubscriber>() { p2, gameLog}); 

        p1.Deck.Add(card);
        p1.Deck[0].PerformAction();
    }
}
