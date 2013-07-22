/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using OpenMetaverse;
using OpenSim.Framework;
using OpenSim.Region.Framework.Scenes;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.ScriptEngine.Shared;
using OpenSim.Region.ScriptEngine.Interfaces;
using log4net;

namespace OpenSim.Region.ScriptEngine.XEngine
{
    /// <summary>
    /// Prepares events so they can be directly executed upon a script by EventQueueManager, then queues it.
    /// </summary>
    public class EventManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private XEngine myScriptEngine;

        public EventManager(XEngine _ScriptEngine)
        {
            myScriptEngine = _ScriptEngine;

            m_log.Info("[XEngine] Hooking up to server events");
            myScriptEngine.World.EventManager.OnAttach += attach;
            myScriptEngine.World.EventManager.OnObjectGrab += touch_start;
            myScriptEngine.World.EventManager.OnObjectGrabbing += touch;
            myScriptEngine.World.EventManager.OnObjectDeGrab += touch_end;
            myScriptEngine.World.EventManager.OnScriptChangedEvent += changed;
            myScriptEngine.World.EventManager.OnScriptAtTargetEvent += at_target;
            myScriptEngine.World.EventManager.OnScriptNotAtTargetEvent += not_at_target;
            myScriptEngine.World.EventManager.OnScriptAtRotTargetEvent += at_rot_target;
            myScriptEngine.World.EventManager.OnScriptNotAtRotTargetEvent += not_at_rot_target;
            myScriptEngine.World.EventManager.OnScriptControlEvent += control;
            myScriptEngine.World.EventManager.OnScriptColliderStart += collision_start;
            myScriptEngine.World.EventManager.OnScriptColliding += collision;
            myScriptEngine.World.EventManager.OnScriptCollidingEnd += collision_end;
            myScriptEngine.World.EventManager.OnScriptLandColliderStart += land_collision_start;
            myScriptEngine.World.EventManager.OnScriptLandColliding += land_collision;
            myScriptEngine.World.EventManager.OnScriptLandColliderEnd += land_collision_end;
            IMoneyModule money=myScriptEngine.World.RequestModuleInterface<IMoneyModule>();
            if (money != null)
            {
                money.OnObjectPaid+=HandleObjectPaid;
            }
        }

        /// <summary>
        /// When an object gets paid by an avatar and generates the paid event, 
        /// this will pipe it to the script engine
        /// </summary>
        /// <param name="objectID">Object ID that got paid</param>
        /// <param name="agentID">Agent Id that did the paying</param>
        /// <param name="amount">Amount paid</param>
        private void HandleObjectPaid(UUID objectID, UUID agentID,
                int amount)
        {
            // Since this is an event from a shared module, all scenes will
            // get it. But only one has the object in question. The others
            // just ignore it.
            //
            SceneObjectPart part =
                    myScriptEngine.World.GetSceneObjectPart(objectID);

            if (part == null)
                return;

            m_log.Debug("Paid: " + objectID + " from " + agentID + ", amount " + amount);

            part = part.ParentGroup.RootPart;
            money(part.LocalId, agentID, amount);
        }

        /// <summary>
        /// Handles piping the proper stuff to The script engine for touching
        /// Including DetectedParams
        /// </summary>
        /// <param name="localID"></param>
        /// <param name="originalID"></param>
        /// <param name="offsetPos"></param>
        /// <param name="remoteClient"></param>
        /// <param name="surfaceArgs"></param>
        public void touch_start(uint localID, uint originalID, Vector3 offsetPos,
                IClientAPI remoteClient, SurfaceTouchEventArgs surfaceArgs)
        {
            // Add to queue for all scripts in ObjectID object
            DetectParams[] det = new DetectParams[1];
            det[0] = new DetectParams();
            det[0].Key = remoteClient.AgentId;
            det[0].Populate(myScriptEngine.World);

            if (originalID == 0)
            {
                SceneObjectPart part = myScriptEngine.World.GetSceneObjectPart(localID);
                if (part == null)
                    return;

                det[0].LinkNum = part.LinkNum;
            }
            else
            {
                SceneObjectPart originalPart = myScriptEngine.World.GetSceneObjectPart(originalID);
                det[0].LinkNum = originalPart.LinkNum;
            }

            if (surfaceArgs != null)
            {
                det[0].SurfaceTouchArgs = surfaceArgs;
            }

            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "touch_start", new Object[] { new LSL_Types.LSLInteger(1) },
                    det));
			/*!< Empiece de modificacion */
            /**
             * Registra la ejecucion del evento touch_start
             */

            /**
             * Calcula la fecha actual.
             */
            string sDate = DateTime.Now.ToString();
            string sDate2 = DateTime.Now.ToString();
            DateTime dDate;
            if (DateTime.TryParse(sDate, out dDate))
            {
                sDate = dDate.ToString("yyyy/MM/dd HH:mm:ss.ff");
            }
            if (DateTime.TryParse(sDate2, out dDate))
            {
                sDate2 = dDate.ToString("yyyy-MM-dd"); //string de la fecha para directorio
            }

            /**
             * Comprueba si existe el directorio "log" dentro del directorio "bin".
             */
            string directoriogeneral = System.IO.Directory.GetCurrentDirectory() + @"\log\Eventos\touchstart\" + sDate2;

            if (!System.IO.Directory.Exists(directoriogeneral))
            {
                System.IO.Directory.CreateDirectory(directoriogeneral);
            }
            /**
             * Anade texto al fichero ".txt"
             */

            string cadena = "";

            if (originalID == 0)
            {
                SceneObjectPart part2 = myScriptEngine.World.GetSceneObjectPart(localID);
                if (part2 != null)
                {
                    cadena += "\nAbsolutePosition: " + part2.AbsolutePosition;
                    //cadena += "\nAcceleration: " + part2.Acceleration;
                    //cadena += "\nAngularVelocity: " + part2.AngularVelocity;
                    //cadena += "\nBaseMask: " + part2.BaseMask;
                    //cadena += "\nCategory: " + part2.Category;
                    //cadena += "\nClickAction: " + part2.ClickAction;
                    //cadena += "\nCollisionSound: " + part2.CollisionSound;
                    //cadena += "\nCollisionSoundVolume: " + part2.CollisionSoundVolume;
                    //cadena += "\nCreationDate: " + part2.CreationDate;
                    cadena += "\nCreatorID: " + part2.CreatorID;
                    //cadena += "\nDamage: " + part2.Damage;
                    cadena += "\nDescription: " + part2.Description;
                    //cadena += "\nEveryoneMask: " + part2.EveryoneMask;
                    //cadena += "\nExpires: " + part2.Expires;
                    //cadena += "\nFolderID: " + part2.FolderID;
                    //cadena += "\nGroupID: " + part2.GroupID;
                    //cadena += "\nGroupMask: " + part2.GroupMask;
                    cadena += "\nGroupPosition: " + part2.GroupPosition;
                    //cadena += "\nInventorySerial: " + part2.InventorySerial;
                    cadena += "\nLastOwnerID: " + part2.LastOwnerID;
                    cadena += "\nLinkNum: " + part2.LinkNum;
                    //cadena += "\nLocalId: " + part2.LocalId;
                    //cadena += "\nLinkNum: " + part2.LinkNum;
                    cadena += "\nMaterial: " + part2.Material;
                    cadena += "\nName: " + part2.Name;
                    //cadena += "\nNextOwnerMask: " + part2.NextOwnerMask;
                    //cadena += "\nObjectFlags: " + part2.ObjectFlags;
                    //cadena += "\nObjectSaleType: " + part2.ObjectSaleType;
                    //cadena += "\nOffsetPosition: " + part2.OffsetPosition;
                    //cadena += "\noriginalID: " + originalID;
                    cadena += "\nOwnerID: " + part2.OwnerID;
                    //cadena += "\nOwnerMask: " + part2.OwnerMask;
                    //cadena += "\nOwnershipCost: " + part2.OwnershipCost;
                    //cadena += "\nParentGroup: " + part2.ParentGroup;
                    //cadena += "\nParentID: " + part2.ParentID;
                    //cadena += "\nParentUUID: " + part2.ParentUUID;
                    //cadena += "\nParticleSystem: " + part2.ParticleSystem;
                    //cadena += "\nPassTouches: " + part2.PassTouches;
                    //cadena += "\nRegionHandle: " + part2.RegionHandle;
                    //cadena += "\nRegionID: " + part2.RegionID;
                    //cadena += "\nRezzed: " + part2.Rezzed;
                    //cadena += "\nRotationOffset: " + part2.RotationOffset;
                    //cadena += "\nSalePrice: " + part2.SalePrice;
                    //cadena += "\nScale: " + part2.Scale;
                    //cadena += "\nScriptAccessPin: " + part2.ScriptAccessPin;
                    //cadena += "\nScriptEvents: " + part2.ScriptEvents;
                    //cadena += "\nShape: " + part2.Shape;
                    //cadena += "\nSitAnimation: " + part2.SitAnimation;
                    //cadena += "\nSitName: " + part2.SitName;
                    //cadena += "\nSitTargetAvatar: " + part2.SitTargetAvatar;
                    //cadena += "\nSitTargetOrientation: " + part2.SitTargetOrientation;
                    //cadena += "\nSitTargetOrientationLL: " + part2.SitTargetOrientationLL;
                    //cadena += "\nSitTargetPosition: " + part2.SitTargetPosition;
                    //cadena += "\nSitTargetPositionLL: " + part2.SitTargetPositionLL;
                    //cadena += "\nStopped: " + part2.Stopped;
                    //cadena += "\nTaskInventory: " + part2.TaskInventory;
                    //cadena += "\nText: " + part2.Text;
                    //cadena += "\nTextureAnimation: " + part2.TextureAnimation;
                    //cadena += "\nTouchname: " + part2.TouchName;
                    //cadena += "\nUpdateFlag: " + part2.UpdateFlag;
                    cadena += "\nUUID: " + part2.UUID;
                    //cadena += "\nVelocity: " + part2.Velocity;
                }
            }
            else
            {
                SceneObjectPart originalPart2 = myScriptEngine.World.GetSceneObjectPart(originalID);

                cadena += "\nAbsolutePosition: " + originalPart2.AbsolutePosition;
                //cadena += "\nAcceleration: " + originalPart2.Acceleration;
                //cadena += "\nAngularVelocity: " + originalPart2.AngularVelocity;
                //cadena += "\nBaseMask: " + originalPart2.BaseMask;
                //cadena += "\nCategory: " + originalPart2.Category;
                //cadena += "\nClickAction: " + originalPart2.ClickAction;
                //cadena += "\nCollisionSound: " + originalPart2.CollisionSound;
                //cadena += "\nCollisionSoundVolume: " + originalPart2.CollisionSoundVolume;
                //cadena += "\nCreationDate: " + originalPart2.CreationDate;
                cadena += "\nCreatorID: " + originalPart2.CreatorID;
                //cadena += "\nDamage: " + originalPart2.Damage;
                cadena += "\nDescription: " + originalPart2.Description;
                //cadena += "\nEveryoneMask: " + originalPart2.EveryoneMask;
                //cadena += "\nExpires: " + originalPart2.Expires;
                //cadena += "\nFolderID: " + originalPart2.FolderID;
                //cadena += "\nGroupID: " + originalPart2.GroupID;
                //cadena += "\nGroupMask: " + originalPart2.GroupMask;
                cadena += "\nGroupPosition: " + originalPart2.GroupPosition;
                //cadena += "\nInventorySerial: " + originalPart2.InventorySerial;
                cadena += "\nLastOwnerID: " + originalPart2.LastOwnerID;
                cadena += "\nLinkNum: " + originalPart2.LinkNum;
                //cadena += "\nlocalID: " + localID;
                //cadena += "\nLocalId: " + originalPart2.LocalId;
                cadena += "\nMaterial: " + originalPart2.Material;
                cadena += "\nName: " + originalPart2.Name;
                //cadena += "\nNextOwnerMask: " + originalPart2.NextOwnerMask;
                //cadena += "\nOffsetPosition: " + originalPart2.OffsetPosition;
                //cadena += "\nObjectFlags: " + originalPart2.ObjectFlags;
                //cadena += "\nObjectSaleType: " + originalPart2.ObjectSaleType;
                //cadena += "\nLinkNum: " + originalPart2.LinkNum;
                cadena += "\nOwnerID: " + originalPart2.OwnerID;
                //cadena += "\nOwnerMask: " + originalPart2.OwnerMask;
                //cadena += "\nOwnershipCost: " + originalPart2.OwnershipCost;
                //cadena += "\nParentUUID: " + originalPart2.ParentUUID;
                //cadena += "\nParentGroup: " + originalPart2.ParentGroup;
                //cadena += "\nParentID: " + originalPart2.ParentID;
                //cadena += "\nParticleSystem: " + originalPart2.ParticleSystem;
                //cadena += "\nPassTouches: " + originalPart2.PassTouches;
                //cadena += "\nRegionID: " + originalPart2.RegionID;
                //cadena += "\nRegionHandle: " + originalPart2.RegionHandle;
                //cadena += "\nRezzed: " + originalPart2.Rezzed;
                //cadena += "\nRotationOffset: " + originalPart2.RotationOffset;
                //cadena += "\nSalePrice: " + originalPart2.SalePrice;
                //cadena += "\nScale: " + originalPart2.Scale;
                //cadena += "\nScriptAccessPin: " + originalPart2.ScriptAccessPin;
                //cadena += "\nScriptEvents: " + originalPart2.ScriptEvents;
                //cadena += "\nShape: " + originalPart2.Shape;
                //cadena += "\nSitAnimation: " + originalPart2.SitAnimation;
                //cadena += "\nSitName: " + originalPart2.SitName;
                //cadena += "\nSitTargetAvatar: " + originalPart2.SitTargetAvatar;
                //cadena += "\nSitTargetOrientation: " + originalPart2.SitTargetOrientation;
                //cadena += "\nSitTargetOrientationLL: " + originalPart2.SitTargetOrientationLL;
                //cadena += "\nSitTargetPosition: " + originalPart2.SitTargetPosition;
                //cadena += "\nSitTargetPositionLL: " + originalPart2.SitTargetPositionLL;
                //cadena += "\nStopped: " + originalPart2.Stopped;
                //cadena += "\nTaskInventory: " + originalPart2.TaskInventory;
                //cadena += "\nText: " + originalPart2.Text;
                //cadena += "\nTextureAnimation: " + originalPart2.TextureAnimation;
                //cadena += "\nTouchName: " + originalPart2.TouchName;
                cadena += "\nUUID: " + originalPart2.UUID;
                //cadena += "\nUpdateFlag: " + originalPart2.UpdateFlag;
                //cadena += "\nVelocity: " + originalPart2.Velocity;

            }

            //cadena += "\noffsetPos: " + offsetPos;
            //cadena += "\nremoteClient.AgentId: " + remoteClient.AgentId;
            //cadena += "\nsurfaceArgs: " + surfaceArgs;
            cadena += "\n^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\n";


            string path = directoriogeneral + "\\" + remoteClient.AgentId + ".txt";
            var perm = new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.Append, path);
            int estado = 0;
            System.IO.StreamWriter sw;
            cadena = "[" + sDate + "]  " + cadena;
            while (estado == 0)
            {
                //comprobar que se puede acceder al archivo
                try
                {
                    sw = new System.IO.StreamWriter(path, true);
                    lock (sw)
                    {
                        sw.WriteLine(cadena);
                        sw.Close();
                    }
                    estado = 1;
                }
                catch { }
            }

            /*!< Fin de modificacion */
        }

        public void touch(uint localID, uint originalID, Vector3 offsetPos,
                IClientAPI remoteClient, SurfaceTouchEventArgs surfaceArgs)
        {
            // Add to queue for all scripts in ObjectID object
            DetectParams[] det = new DetectParams[1];
            det[0] = new DetectParams();
            det[0].Key = remoteClient.AgentId;
            det[0].Populate(myScriptEngine.World);
            det[0].OffsetPos = new LSL_Types.Vector3(offsetPos.X,
                                                     offsetPos.Y,
                                                     offsetPos.Z);

            if (originalID == 0)
            {
                SceneObjectPart part = myScriptEngine.World.GetSceneObjectPart(localID);
                if (part == null)
                    return;

                det[0].LinkNum = part.LinkNum;
            }
            else
            {
                SceneObjectPart originalPart = myScriptEngine.World.GetSceneObjectPart(originalID);
                det[0].LinkNum = originalPart.LinkNum;
            }
            if (surfaceArgs != null)
            {
                det[0].SurfaceTouchArgs = surfaceArgs;
            }

            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "touch", new Object[] { new LSL_Types.LSLInteger(1) },
                    det));
			/*!< Empiece de modificacion */
            /**
             * Registra la ejecucion del evento touch.
             */

            /**
             * Calcula la fecha actual.
             */
            string sDate = DateTime.Now.ToString();
            string sDate2 = DateTime.Now.ToString();
            DateTime dDate;
            if (DateTime.TryParse(sDate, out dDate))
            {
                sDate = dDate.ToString("yyyy/MM/dd HH:mm:ss.ff");
            }
            if (DateTime.TryParse(sDate2, out dDate))
            {
                sDate2 = dDate.ToString("yyyy-MM-dd"); //string de la fecha para directorio
            }

            /**
             * Comprueba si existe el directorio "log\Eventos\touch" dentro del directorio "bin".
             */
            string directoriogeneral = System.IO.Directory.GetCurrentDirectory() + @"\log\Eventos\touch\" + sDate2;

            if (!System.IO.Directory.Exists(directoriogeneral))
            {
                System.IO.Directory.CreateDirectory(directoriogeneral);
            }
            /**
             * Anade texto al fichero ".txt"
             */

            string cadena = "";

            if (originalID == 0)
            {
                SceneObjectPart part2 = myScriptEngine.World.GetSceneObjectPart(localID);
                if (part2 != null)
                {
                    cadena += "\nAbsolutePosition: " + part2.AbsolutePosition;
                    //cadena += "\nAcceleration: " + part2.Acceleration;
                    //cadena += "\nAngularVelocity: " + part2.AngularVelocity;
                    //cadena += "\nBaseMask: " + part2.BaseMask;
                    //cadena += "\nCategory: " + part2.Category;
                    //cadena += "\nClickAction: " + part2.ClickAction;
                    //cadena += "\nCollisionSound: " + part2.CollisionSound;
                    //cadena += "\nCollisionSoundVolume: " + part2.CollisionSoundVolume;
                    //cadena += "\nCreationDate: " + part2.CreationDate;
                    cadena += "\nCreatorID: " + part2.CreatorID;
                    //cadena += "\nDamage: " + part2.Damage;
                    cadena += "\nDescription: " + part2.Description;
                    //cadena += "\nEveryoneMask: " + part2.EveryoneMask;
                    //cadena += "\nExpires: " + part2.Expires;
                    //cadena += "\nFolderID: " + part2.FolderID;
                    //cadena += "\nGroupID: " + part2.GroupID;
                    //cadena += "\nGroupMask: " + part2.GroupMask;
                    cadena += "\nGroupPosition: " + part2.GroupPosition;
                    //cadena += "\nInventorySerial: " + part2.InventorySerial;
                    cadena += "\nLastOwnerID: " + part2.LastOwnerID;
                    cadena += "\nLinkNum: " + part2.LinkNum;
                    //cadena += "\nLocalId: " + part2.LocalId;
                    //cadena += "\nLinkNum: " + part2.LinkNum;
                    cadena += "\nMaterial: " + part2.Material;
                    cadena += "\nName: " + part2.Name;
                    //cadena += "\nNextOwnerMask: " + part2.NextOwnerMask;
                    //cadena += "\nObjectFlags: " + part2.ObjectFlags;
                    //cadena += "\nObjectSaleType: " + part2.ObjectSaleType;
                    //cadena += "\nOffsetPosition: " + part2.OffsetPosition;
                    cadena += "\noriginalID: " + originalID;
                    cadena += "\nOwnerID: " + part2.OwnerID;
                    //cadena += "\nOwnerMask: " + part2.OwnerMask;
                    //cadena += "\nOwnershipCost: " + part2.OwnershipCost;
                    //cadena += "\nParentGroup: " + part2.ParentGroup;
                    //cadena += "\nParentID: " + part2.ParentID;
                    //cadena += "\nParentUUID: " + part2.ParentUUID;
                    //cadena += "\nParticleSystem: " + part2.ParticleSystem;
                    //cadena += "\nPassTouches: " + part2.PassTouches;
                    //cadena += "\nRegionHandle: " + part2.RegionHandle;
                    //cadena += "\nRegionID: " + part2.RegionID;
                    //cadena += "\nRezzed: " + part2.Rezzed;
                    //cadena += "\nRotationOffset: " + part2.RotationOffset;
                    //cadena += "\nSalePrice: " + part2.SalePrice;
                    //cadena += "\nScale: " + part2.Scale;
                    //cadena += "\nScriptAccessPin: " + part2.ScriptAccessPin;
                    //cadena += "\nScriptEvents: " + part2.ScriptEvents;
                    //cadena += "\nShape: " + part2.Shape;
                    //cadena += "\nSitAnimation: " + part2.SitAnimation;
                    //cadena += "\nSitName: " + part2.SitName;
                    //cadena += "\nSitTargetAvatar: " + part2.SitTargetAvatar;
                    //cadena += "\nSitTargetOrientation: " + part2.SitTargetOrientation;
                    //cadena += "\nSitTargetOrientationLL: " + part2.SitTargetOrientationLL;
                    //cadena += "\nSitTargetPosition: " + part2.SitTargetPosition;
                    //cadena += "\nSitTargetPositionLL: " + part2.SitTargetPositionLL;
                    //cadena += "\nStopped: " + part2.Stopped;
                    //cadena += "\nTaskInventory: " + part2.TaskInventory;
                    //cadena += "\nText: " + part2.Text;
                    //cadena += "\nTextureAnimation: " + part2.TextureAnimation;
                    //cadena += "\nTouchname: " + part2.TouchName;
                    //cadena += "\nUpdateFlag: " + part2.UpdateFlag;
                    cadena += "\nUUID: " + part2.UUID;
                    //cadena += "\nVelocity: " + part2.Velocity;

                }
            }
            else
            {
                SceneObjectPart originalPart2 = myScriptEngine.World.GetSceneObjectPart(originalID);

                cadena += "\nAbsolutePosition: " + originalPart2.AbsolutePosition;
                //cadena += "\nAcceleration: " + originalPart2.Acceleration;
                //cadena += "\nAngularVelocity: " + originalPart2.AngularVelocity;
                //cadena += "\nBaseMask: " + originalPart2.BaseMask;
                //cadena += "\nCategory: " + originalPart2.Category;
                //cadena += "\nClickAction: " + originalPart2.ClickAction;
                //cadena += "\nCollisionSound: " + originalPart2.CollisionSound;
                //cadena += "\nCollisionSoundVolume: " + originalPart2.CollisionSoundVolume;
                //cadena += "\nCreationDate: " + originalPart2.CreationDate;
                cadena += "\nCreatorID: " + originalPart2.CreatorID;
                //cadena += "\nDamage: " + originalPart2.Damage;
                cadena += "\nDescription: " + originalPart2.Description;
                //cadena += "\nEveryoneMask: " + originalPart2.EveryoneMask;
                //cadena += "\nExpires: " + originalPart2.Expires;
                //cadena += "\nFolderID: " + originalPart2.FolderID;
                //cadena += "\nGroupID: " + originalPart2.GroupID;
                //cadena += "\nGroupMask: " + originalPart2.GroupMask;
                cadena += "\nGroupPosition: " + originalPart2.GroupPosition;
                //cadena += "\nInventorySerial: " + originalPart2.InventorySerial;
                cadena += "\nLastOwnerID: " + originalPart2.LastOwnerID;
                cadena += "\nLinkNum: " + originalPart2.LinkNum;
                //cadena += "\nlocalID: " + localID;
                //cadena += "\nLocalId: " + originalPart2.LocalId;
                cadena += "\nMaterial: " + originalPart2.Material;
                cadena += "\nName: " + originalPart2.Name;
                //cadena += "\nNextOwnerMask: " + originalPart2.NextOwnerMask;
                //cadena += "\nOffsetPosition: " + originalPart2.OffsetPosition;
                //cadena += "\nObjectFlags: " + originalPart2.ObjectFlags;
                //cadena += "\nObjectSaleType: " + originalPart2.ObjectSaleType;
                //cadena += "\nLinkNum: " + originalPart2.LinkNum;
                cadena += "\nOwnerID: " + originalPart2.OwnerID;
                //cadena += "\nOwnerMask: " + originalPart2.OwnerMask;
                //cadena += "\nOwnershipCost: " + originalPart2.OwnershipCost;
                //cadena += "\nParentUUID: " + originalPart2.ParentUUID;
                //cadena += "\nParentGroup: " + originalPart2.ParentGroup;
                //cadena += "\nParentID: " + originalPart2.ParentID;
                //cadena += "\nParticleSystem: " + originalPart2.ParticleSystem;
                //cadena += "\nPassTouches: " + originalPart2.PassTouches;
                //cadena += "\nRegionID: " + originalPart2.RegionID;
                //cadena += "\nRegionHandle: " + originalPart2.RegionHandle;
                //cadena += "\nRezzed: " + originalPart2.Rezzed;
                //cadena += "\nRotationOffset: " + originalPart2.RotationOffset;
                //cadena += "\nSalePrice: " + originalPart2.SalePrice;
                //cadena += "\nScale: " + originalPart2.Scale;
                //cadena += "\nScriptAccessPin: " + originalPart2.ScriptAccessPin;
                //cadena += "\nScriptEvents: " + originalPart2.ScriptEvents;
                //cadena += "\nShape: " + originalPart2.Shape;
                //cadena += "\nSitAnimation: " + originalPart2.SitAnimation;
                //cadena += "\nSitName: " + originalPart2.SitName;
                //cadena += "\nSitTargetAvatar: " + originalPart2.SitTargetAvatar;
                //cadena += "\nSitTargetOrientation: " + originalPart2.SitTargetOrientation;
                //cadena += "\nSitTargetOrientationLL: " + originalPart2.SitTargetOrientationLL;
                //cadena += "\nSitTargetPosition: " + originalPart2.SitTargetPosition;
                //cadena += "\nSitTargetPositionLL: " + originalPart2.SitTargetPositionLL;
                //cadena += "\nStopped: " + originalPart2.Stopped;
                //cadena += "\nTaskInventory: " + originalPart2.TaskInventory;
                //cadena += "\nText: " + originalPart2.Text;
                //cadena += "\nTextureAnimation: " + originalPart2.TextureAnimation;
                //cadena += "\nTouchName: " + originalPart2.TouchName;
                cadena += "\nUUID: " + originalPart2.UUID;
                //cadena += "\nUpdateFlag: " + originalPart2.UpdateFlag;
                //cadena += "\nVelocity: " + originalPart2.Velocity;

            }
            //cadena += "\noffsetPos: " + offsetPos;
            //cadena += "\nremoteClient.AgentId: " + remoteClient.AgentId;
            cadena += "\n^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\n";

            string path = directoriogeneral + "\\" + remoteClient.AgentId + ".txt";
            var perm = new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.Append, path);
            int estado = 0;
            System.IO.StreamWriter sw;
            cadena = "[" + sDate + "]  " + cadena;
            while (estado == 0)
            {
                //comprobar que se puede acceder al archivo
                try
                {
                    sw = new System.IO.StreamWriter(path, true);
                    lock (sw)
                    {
                        sw.WriteLine(cadena);
                        sw.Close();
                    }
                    estado = 1;
                }
                catch { }
            }

            /*!< Fin de modificacion */
        }

        public void touch_end(uint localID, uint originalID, IClientAPI remoteClient,
                              SurfaceTouchEventArgs surfaceArgs)
        {
            // Add to queue for all scripts in ObjectID object
            DetectParams[] det = new DetectParams[1];
            det[0] = new DetectParams();
            det[0].Key = remoteClient.AgentId;
            det[0].Populate(myScriptEngine.World);

            if (originalID == 0)
            {
                SceneObjectPart part = myScriptEngine.World.GetSceneObjectPart(localID);
                if (part == null)
                    return;

                det[0].LinkNum = part.LinkNum;
            }
            else
            {
                SceneObjectPart originalPart = myScriptEngine.World.GetSceneObjectPart(originalID);
                det[0].LinkNum = originalPart.LinkNum;
            }

            if (surfaceArgs != null)
            {
                det[0].SurfaceTouchArgs = surfaceArgs;
            }

            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "touch_end", new Object[] { new LSL_Types.LSLInteger(1) },
                    det));
			/*!< Empiece de modificacion */
            /**
             * Registra la ejecucion del evento touch_end
             */

            /**
             * Calcula la fecha actual.
             */
            string sDate = DateTime.Now.ToString();
            string sDate2 = DateTime.Now.ToString();
            DateTime dDate;
            if (DateTime.TryParse(sDate, out dDate))
            {
                sDate = dDate.ToString("yyyy/MM/dd HH:mm:ss.ff");
            }
            if (DateTime.TryParse(sDate2, out dDate))
            {
                sDate2 = dDate.ToString("yyyy-MM-dd"); //string de la fecha para directorio
            }

            /**
             * Comprueba si existe el directorio "log" dentro del directorio "bin".
             */
            string directoriogeneral = System.IO.Directory.GetCurrentDirectory() + @"\log\Eventos\touchend\" + sDate2;

            if (!System.IO.Directory.Exists(directoriogeneral))
            {
                System.IO.Directory.CreateDirectory(directoriogeneral);
            }
            /**
             * Anade texto al fichero ".txt"
             */

            string cadena = "";

            if (originalID == 0)
            {
                SceneObjectPart part2 = myScriptEngine.World.GetSceneObjectPart(localID);
                if (part2 != null)
                {
                    cadena += "\nAbsolutePosition: " + part2.AbsolutePosition;
                    //cadena += "\nAcceleration: " + part2.Acceleration;
                    //cadena += "\nAngularVelocity: " + part2.AngularVelocity;
                    //cadena += "\nBaseMask: " + part2.BaseMask;
                    //cadena += "\nCategory: " + part2.Category;
                    //cadena += "\nClickAction: " + part2.ClickAction;
                    //cadena += "\nCollisionSound: " + part2.CollisionSound;
                    //cadena += "\nCollisionSoundVolume: " + part2.CollisionSoundVolume;
                    //cadena += "\nCreationDate: " + part2.CreationDate;
                    cadena += "\nCreatorID: " + part2.CreatorID;
                    //cadena += "\nDamage: " + part2.Damage;
                    cadena += "\nDescription: " + part2.Description;
                    //cadena += "\nEveryoneMask: " + part2.EveryoneMask;
                    //cadena += "\nExpires: " + part2.Expires;
                    //cadena += "\nFolderID: " + part2.FolderID;
                    //cadena += "\nGroupID: " + part2.GroupID;
                    //cadena += "\nGroupMask: " + part2.GroupMask;
                    cadena += "\nGroupPosition: " + part2.GroupPosition;
                    //cadena += "\nInventorySerial: " + part2.InventorySerial;
                    cadena += "\nLastOwnerID: " + part2.LastOwnerID;
                    cadena += "\nLinkNum: " + part2.LinkNum;
                    //cadena += "\nLocalId: " + part2.LocalId;
                    //cadena += "\nLinkNum: " + part2.LinkNum;
                    cadena += "\nMaterial: " + part2.Material;
                    cadena += "\nName: " + part2.Name;
                    //cadena += "\nNextOwnerMask: " + part2.NextOwnerMask;
                    //cadena += "\nObjectFlags: " + part2.ObjectFlags;
                    //cadena += "\nObjectSaleType: " + part2.ObjectSaleType;
                    //cadena += "\nOffsetPosition: " + part2.OffsetPosition;
                    //cadena += "\noriginalID: " + originalID;
                    cadena += "\nOwnerID: " + part2.OwnerID;
                    //cadena += "\nOwnerMask: " + part2.OwnerMask;
                    //cadena += "\nOwnershipCost: " + part2.OwnershipCost;
                    //cadena += "\nParentGroup: " + part2.ParentGroup;
                    //cadena += "\nParentID: " + part2.ParentID;
                    //cadena += "\nParentUUID: " + part2.ParentUUID;
                    //cadena += "\nParticleSystem: " + part2.ParticleSystem;
                    //cadena += "\nPassTouches: " + part2.PassTouches;
                    //cadena += "\nRegionHandle: " + part2.RegionHandle;
                    //cadena += "\nRegionID: " + part2.RegionID;
                    //cadena += "\nRezzed: " + part2.Rezzed;
                    //cadena += "\nRotationOffset: " + part2.RotationOffset;
                    //cadena += "\nSalePrice: " + part2.SalePrice;
                    //cadena += "\nScale: " + part2.Scale;
                    //cadena += "\nScriptAccessPin: " + part2.ScriptAccessPin;
                    //cadena += "\nScriptEvents: " + part2.ScriptEvents;
                    //cadena += "\nShape: " + part2.Shape;
                    //cadena += "\nSitAnimation: " + part2.SitAnimation;
                    //cadena += "\nSitName: " + part2.SitName;
                    //cadena += "\nSitTargetAvatar: " + part2.SitTargetAvatar;
                    //cadena += "\nSitTargetOrientation: " + part2.SitTargetOrientation;
                    //cadena += "\nSitTargetOrientationLL: " + part2.SitTargetOrientationLL;
                    //cadena += "\nSitTargetPosition: " + part2.SitTargetPosition;
                    //cadena += "\nSitTargetPositionLL: " + part2.SitTargetPositionLL;
                    //cadena += "\nStopped: " + part2.Stopped;
                    //cadena += "\nTaskInventory: " + part2.TaskInventory;
                    //cadena += "\nText: " + part2.Text;
                    //cadena += "\nTextureAnimation: " + part2.TextureAnimation;
                    //cadena += "\nTouchname: " + part2.TouchName;
                    //cadena += "\nUpdateFlag: " + part2.UpdateFlag;
                    cadena += "\nUUID: " + part2.UUID;
                    //cadena += "\nVelocity: " + part2.Velocity;

                }
            }
            else
            {
                SceneObjectPart originalPart2 = myScriptEngine.World.GetSceneObjectPart(originalID);

                cadena += "\nAbsolutePosition: " + originalPart2.AbsolutePosition;
                //cadena += "\nAcceleration: " + originalPart2.Acceleration;
                //cadena += "\nAngularVelocity: " + originalPart2.AngularVelocity;
                //cadena += "\nBaseMask: " + originalPart2.BaseMask;
                //cadena += "\nCategory: " + originalPart2.Category;
                //cadena += "\nClickAction: " + originalPart2.ClickAction;
                //cadena += "\nCollisionSound: " + originalPart2.CollisionSound;
                //cadena += "\nCollisionSoundVolume: " + originalPart2.CollisionSoundVolume;
                //cadena += "\nCreationDate: " + originalPart2.CreationDate;
                cadena += "\nCreatorID: " + originalPart2.CreatorID;
                //cadena += "\nDamage: " + originalPart2.Damage;
                cadena += "\nDescription: " + originalPart2.Description;
                //cadena += "\nEveryoneMask: " + originalPart2.EveryoneMask;
                //cadena += "\nExpires: " + originalPart2.Expires;
                //cadena += "\nFolderID: " + originalPart2.FolderID;
                //cadena += "\nGroupID: " + originalPart2.GroupID;
                //cadena += "\nGroupMask: " + originalPart2.GroupMask;
                //cadena += "\nGroupPosition: " + originalPart2.GroupPosition;
                //cadena += "\nInventorySerial: " + originalPart2.InventorySerial;
                cadena += "\nLastOwnerID: " + originalPart2.LastOwnerID;
                cadena += "\nLinkNum: " + originalPart2.LinkNum;
                //cadena += "\nlocalID: " + localID;
                //cadena += "\nLocalId: " + originalPart2.LocalId;
                cadena += "\nMaterial: " + originalPart2.Material;
                cadena += "\nName: " + originalPart2.Name;
                //cadena += "\nNextOwnerMask: " + originalPart2.NextOwnerMask;
                //cadena += "\nOffsetPosition: " + originalPart2.OffsetPosition;
                //cadena += "\nObjectFlags: " + originalPart2.ObjectFlags;
                //cadena += "\nObjectSaleType: " + originalPart2.ObjectSaleType;
                //cadena += "\nLinkNum: " + originalPart2.LinkNum;
                cadena += "\nOwnerID: " + originalPart2.OwnerID;
                //cadena += "\nOwnerMask: " + originalPart2.OwnerMask;
                //cadena += "\nOwnershipCost: " + originalPart2.OwnershipCost;
                //cadena += "\nParentUUID: " + originalPart2.ParentUUID;
                //cadena += "\nParentGroup: " + originalPart2.ParentGroup;
                //cadena += "\nParentID: " + originalPart2.ParentID;
                //cadena += "\nParticleSystem: " + originalPart2.ParticleSystem;
                //cadena += "\nPassTouches: " + originalPart2.PassTouches;
                //cadena += "\nRegionID: " + originalPart2.RegionID;
                //cadena += "\nRegionHandle: " + originalPart2.RegionHandle;
                //cadena += "\nRezzed: " + originalPart2.Rezzed;
                //cadena += "\nRotationOffset: " + originalPart2.RotationOffset;
                //cadena += "\nSalePrice: " + originalPart2.SalePrice;
                //cadena += "\nScale: " + originalPart2.Scale;
                //cadena += "\nScriptAccessPin: " + originalPart2.ScriptAccessPin;
                //cadena += "\nScriptEvents: " + originalPart2.ScriptEvents;
                //cadena += "\nShape: " + originalPart2.Shape;
                //cadena += "\nSitAnimation: " + originalPart2.SitAnimation;
                //cadena += "\nSitName: " + originalPart2.SitName;
                //cadena += "\nSitTargetAvatar: " + originalPart2.SitTargetAvatar;
                //cadena += "\nSitTargetOrientation: " + originalPart2.SitTargetOrientation;
                //cadena += "\nSitTargetOrientationLL: " + originalPart2.SitTargetOrientationLL;
                //cadena += "\nSitTargetPosition: " + originalPart2.SitTargetPosition;
                //cadena += "\nSitTargetPositionLL: " + originalPart2.SitTargetPositionLL;
                //cadena += "\nStopped: " + originalPart2.Stopped;
                //cadena += "\nTaskInventory: " + originalPart2.TaskInventory;
                //cadena += "\nText: " + originalPart2.Text;
                //cadena += "\nTextureAnimation: " + originalPart2.TextureAnimation;
                //cadena += "\nTouchName: " + originalPart2.TouchName;
                cadena += "\nUUID: " + originalPart2.UUID;
                //cadena += "\nUpdateFlag: " + originalPart2.UpdateFlag;
                //cadena += "\nVelocity: " + originalPart2.Velocity;

            }

            //cadena += "\nremoteClient.AgentId: " + remoteClient.AgentId;
            //cadena += "\nsurfaceArgs: " + surfaceArgs;
            cadena += "\n^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\n";

            string path = directoriogeneral + "\\" + remoteClient.AgentId + ".txt";
            var perm = new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.Append, path);
            int estado = 0;
            System.IO.StreamWriter sw;
            cadena = "[" + sDate + "]  " + cadena;
            while (estado == 0)
            {
                //comprobar que se puede acceder al archivo
                try
                {
                    sw = new System.IO.StreamWriter(path, true);
                    lock (sw)
                    {
                        sw.WriteLine(cadena);
                        sw.Close();
                    }
                    estado = 1;
                }
                catch { }
            }

            /*!< Fin de modificacion */
        }

        public void changed(uint localID, uint change)
        {
            // Add to queue for all scripts in localID, Object pass change.
            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "changed",new object[] { new LSL_Types.LSLInteger(change) },
                    new DetectParams[0]));
        }

        // state_entry: not processed here
        // state_exit: not processed here

        public void money(uint localID, UUID agentID, int amount)
        {
            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "money", new object[] {
                    new LSL_Types.LSLString(agentID.ToString()),
                    new LSL_Types.LSLInteger(amount) },
                    new DetectParams[0]));
        }

        public void collision_start(uint localID, ColliderArgs col)
        {
            // Add to queue for all scripts in ObjectID object
            List<DetectParams> det = new List<DetectParams>();

            foreach (DetectedObject detobj in col.Colliders)
            {
                DetectParams d = new DetectParams();
                d.Key =detobj.keyUUID;
                d.Populate(myScriptEngine.World);
                det.Add(d);
            }

            if (det.Count > 0)
                myScriptEngine.PostObjectEvent(localID, new EventParams(
                        "collision_start",
                        new Object[] { new LSL_Types.LSLInteger(det.Count) },
                        det.ToArray()));
        }

        public void collision(uint localID, ColliderArgs col)
        {
            // Add to queue for all scripts in ObjectID object
            List<DetectParams> det = new List<DetectParams>();

            foreach (DetectedObject detobj in col.Colliders)
            {
                DetectParams d = new DetectParams();
                d.Key =detobj.keyUUID;
                d.Populate(myScriptEngine.World);
                det.Add(d);
            }

            if (det.Count > 0)
                myScriptEngine.PostObjectEvent(localID, new EventParams(
                        "collision", new Object[] { new LSL_Types.LSLInteger(det.Count) },
                        det.ToArray()));
        }

        public void collision_end(uint localID, ColliderArgs col)
        {
            // Add to queue for all scripts in ObjectID object
            List<DetectParams> det = new List<DetectParams>();

            foreach (DetectedObject detobj in col.Colliders)
            {
                DetectParams d = new DetectParams();
                d.Key =detobj.keyUUID;
                d.Populate(myScriptEngine.World);
                det.Add(d);
            }

            if (det.Count > 0)
                myScriptEngine.PostObjectEvent(localID, new EventParams(
                        "collision_end",
                        new Object[] { new LSL_Types.LSLInteger(det.Count) },
                        det.ToArray()));
        }

        public void land_collision_start(uint localID, ColliderArgs col)
         {
            List<DetectParams> det = new List<DetectParams>();

            foreach (DetectedObject detobj in col.Colliders)
            {
                DetectParams d = new DetectParams();
                d.Position = new LSL_Types.Vector3(detobj.posVector.X,
                    detobj.posVector.Y,
                    detobj.posVector.Z);
                d.Populate(myScriptEngine.World);
                det.Add(d);
                myScriptEngine.PostObjectEvent(localID, new EventParams(
                        "land_collision_start",
                        new Object[] { new LSL_Types.Vector3(d.Position) },
                        det.ToArray()));
            }

        }

        public void land_collision(uint localID, ColliderArgs col)
        {
            List<DetectParams> det = new List<DetectParams>();

            foreach (DetectedObject detobj in col.Colliders)
            {
                DetectParams d = new DetectParams();
                d.Position = new LSL_Types.Vector3(detobj.posVector.X,
                    detobj.posVector.Y,
                    detobj.posVector.Z);
                d.Populate(myScriptEngine.World);
                det.Add(d);
                myScriptEngine.PostObjectEvent(localID, new EventParams(
                        "land_collision",
                        new Object[] { new LSL_Types.Vector3(d.Position) },
                        det.ToArray()));
            }
        }

        public void land_collision_end(uint localID, ColliderArgs col)
        {
            List<DetectParams> det = new List<DetectParams>();

            foreach (DetectedObject detobj in col.Colliders)
            {
                DetectParams d = new DetectParams();
                d.Position = new LSL_Types.Vector3(detobj.posVector.X,
                    detobj.posVector.Y,
                    detobj.posVector.Z);
                d.Populate(myScriptEngine.World);
                det.Add(d);
                myScriptEngine.PostObjectEvent(localID, new EventParams(
                        "land_collision_end",
                        new Object[] { new LSL_Types.Vector3(d.Position) },
                        det.ToArray()));
            }
         }

        // timer: not handled here
        // listen: not handled here

        public void control(UUID itemID, UUID agentID, uint held, uint change)
        {
            myScriptEngine.PostScriptEvent(itemID, new EventParams(
                    "control",new object[] {
                    new LSL_Types.LSLString(agentID.ToString()),
                    new LSL_Types.LSLInteger(held),
                    new LSL_Types.LSLInteger(change)},
                    new DetectParams[0]));
        }

        public void email(uint localID, UUID itemID, string timeSent,
                string address, string subject, string message, int numLeft)
        {
            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "email",new object[] {
                    new LSL_Types.LSLString(timeSent),
                    new LSL_Types.LSLString(address),
                    new LSL_Types.LSLString(subject),
                    new LSL_Types.LSLString(message),
                    new LSL_Types.LSLInteger(numLeft)},
                    new DetectParams[0]));
        }

        public void at_target(uint localID, uint handle, Vector3 targetpos,
                Vector3 atpos)
        {
            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "at_target", new object[] {
                    new LSL_Types.LSLInteger(handle),
                    new LSL_Types.Vector3(targetpos.X,targetpos.Y,targetpos.Z),
                    new LSL_Types.Vector3(atpos.X,atpos.Y,atpos.Z) },
                    new DetectParams[0]));
        }

        public void not_at_target(uint localID)
        {
            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "not_at_target",new object[0],
                    new DetectParams[0]));
        }

        public void at_rot_target(uint localID, uint handle, Quaternion targetrot,
                Quaternion atrot)
        {
            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "at_rot_target", new object[] {
                    new LSL_Types.LSLInteger(handle),
                    new LSL_Types.Quaternion(targetrot.X,targetrot.Y,targetrot.Z,targetrot.W),
                    new LSL_Types.Quaternion(atrot.X,atrot.Y,atrot.Z,atrot.W) },
                    new DetectParams[0]));
        }

        public void not_at_rot_target(uint localID)
        {
            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "not_at_rot_target",new object[0],
                    new DetectParams[0]));
        }

        // run_time_permissions: not handled here

        public void attach(uint localID, UUID itemID, UUID avatar)
        {
            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "attach",new object[] {
                    new LSL_Types.LSLString(avatar.ToString()) },
                    new DetectParams[0]));
        }

        // dataserver: not handled here
        // link_message: not handled here

        public void moving_start(uint localID, UUID itemID)
        {
            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "moving_start",new object[0],
                    new DetectParams[0]));
        }

        public void moving_end(uint localID, UUID itemID)
        {
            myScriptEngine.PostObjectEvent(localID, new EventParams(
                    "moving_end",new object[0],
                    new DetectParams[0]));
        }

        // object_rez: not handled here
        // remote_data: not handled here
        // http_response: not handled here
    }
}
