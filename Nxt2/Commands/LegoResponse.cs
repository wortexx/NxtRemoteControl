using System;
using System.ComponentModel;
using Nxt2.Common;

namespace Nxt2.Commands
{
    /// <summary>
    /// The base type for all LEGO Responses
    /// </summary>
    [Description("The default LEGO Command Response.")]
    public class LegoResponse : LegoCommand
    {
        #region Private Members
        private LegoErrorCode _legoErrorCode = LegoErrorCode.UnknownStatus;
        #endregion

        #region Static Members
        /// <summary>
        /// The Lego Command Type for a reply
        /// </summary>
        [Description("The Lego Command Type for a reply (response) packet from the LEGO NXT Brick.")]
        public const byte NxtResponse = 0x02;
        #endregion

        /// <summary>
        /// The Expected Lego Response Code
        /// </summary>
        [Description("Identifies the expected LEGO Command Code for this Response.")]
        public LegoCommandCode ExpectedCommandCode;

        /// <summary>
        /// Indicates the error code returned
        /// </summary>
        [Description("Indicates the error code returned.")]
        public LegoErrorCode ErrorCode
        {
            get
            {
                var errorCode = _legoErrorCode;
                if (CommandData != null && CommandData.Length >= 3)
                {
                    try
                    {
                        errorCode = (LegoErrorCode)CommandData[2];
                        int ec;
                        if (Int32.TryParse(errorCode.ToString(), out ec))
                            errorCode = LegoErrorCode.UnknownStatus;
                    }
                    catch
                    {
                        errorCode = LegoErrorCode.UnknownStatus;
                    }
                }
                return errorCode;
            }
            set
            {
                _legoErrorCode = value;

                if (CommandData != null && CommandData.Length >= 3)
                    CommandData[2] = (byte)_legoErrorCode;
            }
        }

        #region Remove [DataMember] from base class fields

        /// <summary>
        /// This is the response.
        /// </summary>
        public override bool RequireResponse
        {
            get { return false; }
            set { base.RequireResponse = false; }
        }

        /// <summary>
        /// The LEGO Response data buffer
        /// </summary>
        [Description("The LEGO Response Data buffer.")]
        public override byte[] CommandData
        {
            get { return internalCommandData; }
            set { internalCommandData = value; }
        }

        /// <summary>
        /// The Expected Response Size
        /// </summary>
        public override int ExpectedResponseSize
        {
            get
            {
                return internalExpectedResponseSize;
            }
            set
            {
                internalExpectedResponseSize = value;
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// A LEGO Command response
        /// </summary>
        public LegoResponse()
        {
            base.TimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Constructor used by inheriting members to initialize the LegoResponse.
        /// </summary>
        /// <param name="expectedResponseSize"></param>
        /// <param name="commandCode"></param>
        public LegoResponse(int expectedResponseSize, LegoCommandCode commandCode)
        {
            this.ExpectedResponseSize = expectedResponseSize;
            this.ExpectedCommandCode = commandCode;
            this.CommandData = null;
            base.TimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Constructor used by inheriting members to initialize the LegoResponse with Data
        /// </summary>
        /// <param name="responseData"></param>
        public LegoResponse(byte[] responseData)
        {
            this.CommandData = responseData;
            this.ExpectedResponseSize = responseData.Length;
            this.ExpectedCommandCode = (LegoCommandCode)responseData[1];
            if (this.CommandData == null)
            {
                this.CommandData = new byte[3];
                this.CommandType = LegoCommandType.Response;
                this.CommandData[1] = (byte)ExpectedCommandCode;
                this.CommandData[2] = 0;
            }
            base.TimeStamp = DateTime.Now;
        }


        /// <summary>
        /// Constructor used by inheriting members to initialize the LegoResponse with Data
        /// </summary>
        /// <param name="expectedResponseSize"></param>
        /// <param name="commandCode"></param>
        /// <param name="responseData"></param>
        public LegoResponse(int expectedResponseSize, LegoCommandCode commandCode, byte[] responseData)
        {
            this.ExpectedResponseSize = expectedResponseSize;
            this.ExpectedCommandCode = commandCode;
            this.CommandData = responseData;
            if (this.CommandData == null)
            {
                this.CommandData = new byte[3];
                this.CommandType = LegoCommandType.Response;
                this.CommandData[1] = (byte)ExpectedCommandCode;
                this.CommandData[2] = 0;
            }
            base.TimeStamp = DateTime.Now;
        }


        /// <summary>
        /// LEGO Response Constructor which matches LEGOCommand initialization parameters
        /// </summary>
        /// <param name="expectedResponseSize"></param>
        /// <param name="responseData"></param>
        public LegoResponse(int expectedResponseSize, params byte[] responseData)
            : base(expectedResponseSize, responseData)
        {
            if (this.CommandData == null)
            {
                this.CommandData = new byte[3];
                this.CommandType = LegoCommandType.Response;
                this.CommandData[1] = (byte)this.ExpectedCommandCode;
                this.CommandData[2] = 0;
            }
            base.TimeStamp = DateTime.Now;
        }

        #endregion

        #region Helper Properties

        /// <summary>
        /// Has Response
        /// </summary>
        [Browsable(false)]
        protected bool HasResponse
        {
            get
            {
                return (this.CommandData != null)
                       && (this.CommandData.Length == this.ExpectedResponseSize)
                       && this.CommandData.Length > 0;
            }
        }

        /// <summary>
        /// The standard acknowledgement from a LEGO Command
        /// </summary>
        [Browsable(false)]
        public virtual bool Success
        {
            get
            {
                if (!HasResponse
                    || this.CommandData.Length < 3)
                    return false;

                return (this.CommandType == LegoCommandType.Response
                        && this.CommandData[1] == (byte)this.ExpectedCommandCode
                        && this.CommandData[2] == 0);
            }
        }

        #endregion

        /// <summary>
        /// Upcast a LegoResponse to it's proper type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ok"></param>
        /// <returns></returns>
        public static T Upcast<T>(LegoResponse ok)
            where T : LegoResponse, new()
        {
            var queryResponse = ok as T ?? new T
                {
                    CommandData = ok.CommandData
                };
            return queryResponse;
        }
    }
}