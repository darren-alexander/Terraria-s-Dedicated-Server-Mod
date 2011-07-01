﻿using System;

namespace Terraria_Server.Messages
{
    public class RequestTileMessage : IMessage
    {
        public Packet GetPacket()
        {
            return Packet.REQUEST_TILE_BLOCK;
        }

        public int? GetRequiredNetMode()
        {
            return 2;
        }

        public void Process(int start, int length, int num, int whoAmI, byte[] readBuffer, byte bufferData)
        {
            int num8 = BitConverter.ToInt32(readBuffer, num);
            num += 4;
            int num9 = BitConverter.ToInt32(readBuffer, num);
            num += 4;

            bool flag3 = !(num8 == -1 
                || num8 < 10 
                || num8 > Main.maxTilesX - 10 
                || num9 == -1
                || num9 < 10
                || num9 > Main.maxTilesY - 10);
            
            int num10 = 1350;
            if (flag3)
            {
                num10 *= 2;
            }

            ServerSock serverSock = Netplay.serverSock[whoAmI];
            if (serverSock.state == 2)
            {
                serverSock.state = 3;
            }

            NetMessage.SendData(9, whoAmI, -1, "Receiving tile data", num10);
            serverSock.statusText2 = "is receiving tile data";
            serverSock.statusMax += num10;
            int sectionX = Netplay.GetSectionX(Main.spawnTileX);
            int sectionY = Netplay.GetSectionY(Main.spawnTileY);

            for (int x = sectionX - 2; x < sectionX + 3; x++)
            {
                for (int y = sectionY - 1; y < sectionY + 2; y++)
                {
                    NetMessage.SendSection(whoAmI, x, y);
                }
            }

            if (flag3)
            {
                num8 = Netplay.GetSectionX(num8);
                num9 = Netplay.GetSectionY(num9);
                for (int num11 = num8 - 2; num11 < num8 + 3; num11++)
                {
                    for (int num12 = num9 - 1; num12 < num9 + 2; num12++)
                    {
                        NetMessage.SendSection(whoAmI, num11, num12);
                    }
                }
                NetMessage.SendData(11, whoAmI, -1, "", num8 - 2, (float)(num9 - 1), (float)(num8 + 2), (float)(num9 + 1));
            }

            NetMessage.SendData(11, whoAmI, -1, "", sectionX - 2, (float)(sectionY - 1), (float)(sectionX + 2), (float)(sectionY + 1));

            //Can't switch to a for each because there are 201 items.
            for (int num13 = 0; num13 < 200; num13++)
            {
                if (Main.item[num13].Active)
                {
                    NetMessage.SendData(21, whoAmI, -1, "", num13);
                    NetMessage.SendData(22, whoAmI, -1, "", num13);
                }
            }
            
            //Can't switch to a for each because there are 1001 NPCs.
            for (int num14 = 0; num14 < 1000; num14++)
            {
                if (Main.npc[num14].active)
                {
                    NetMessage.SendData(23, whoAmI, -1, "", num14);
                }
            }
            NetMessage.SendData(49, whoAmI);
        }
    }
}