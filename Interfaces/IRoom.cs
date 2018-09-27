using System.Collections.Generic;
using castle_grimtol.Models;

namespace castle_grimtol.Interfaces
{
    public interface IRoom
    {
        string Name { get; set; }
        string Description { get; set; }
        List<IItem> Items { get; set; }
        // you don't need dictionary here, just AddRoom and RemoveRoom
        // Dictionary<string, Room> Exits { get; set; }
        void AddRoom(string direction, Room room, bool connectOtherSide = true);
        void RemoveRoom(string direction, bool disconnectOtherSide = true);
        string Locked { get; set; }
        bool Unlock();
    }
}