using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreDemo.Models;
using AspNetCoreDemo.Repositories.Contracts;
using AspNetCoreDemo.Services.Contracts;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCoreDemo.Services
{
    public class SwiftService : ISwiftService
    {
        public readonly ISwiftRepository swiftRepository;

        public SwiftService(ISwiftRepository swiftRepository)
        {
            this.swiftRepository = swiftRepository;
        }

        public async Task<Response<IEnumerable<Swift>>> GetAllAsync()
        {
            var result = new Response<IEnumerable<Swift>>();
           
            var swifts = await this.swiftRepository.GetAllAsync();
            if (!swifts.Any())
            {
                result.IsSuccessful = false;
                result.Message = ServicesConstants.NoRecordsFoundMessage;
                return result;
            }
            result.Data = swifts;
           
            return result;
        }

        public async Task<Response<Swift>> CreateAsync(string filePath)
        {
            var result = new Response<Swift>();

            var parseResult = await ParseSwiftAsync(filePath);
            if (!parseResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = parseResult.Message;
                return result;
            }

            var swift = await this.swiftRepository.CreateAsync(parseResult.Data);
            
            result.Data = swift;
            
            return result;
        }

        public async Task<Response<Swift>> ParseSwiftAsync(string filePath)
        {
            var result = new Response<Swift>();
            var swift = new Swift();

            var resultSwiftFileText = await ReadAllFileText(filePath);
            if (!resultSwiftFileText.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = resultSwiftFileText.Message;
                return result;
            }

            var resultParseSwiftBlocks = await ParseSwiftBlocksAsync(resultSwiftFileText.Data, swift);
            if (!resultParseSwiftBlocks.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = resultParseSwiftBlocks.Message;
                return result;
            }
            swift = resultParseSwiftBlocks.Data;

            var resultParseSwiftTextBlocks = await ParseSwiftTextBlockAsync(resultSwiftFileText.Data, swift);
            if (!resultParseSwiftTextBlocks.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = resultParseSwiftTextBlocks.Message;
                return result;
            }
            swift = resultParseSwiftTextBlocks.Data;

            var resultParseSwiftNarrative = await ParseSwiftNarrativeAsync(resultSwiftFileText.Data, swift);
            if (!resultParseSwiftNarrative.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = resultParseSwiftNarrative.Message;
                return result;
            }
            swift = resultParseSwiftNarrative.Data;

            var resultParseTrailerBlock = await ParseTrailerBlockAsync(resultSwiftFileText.Data, swift);
            if (!resultParseTrailerBlock.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = resultParseTrailerBlock.Message;
                return result;
            }
            swift = resultParseTrailerBlock.Data;
           
            result.Data = swift;

            return result;
        }

        private async Task<Response<string>> ReadAllFileText(string filePath)
        {
            var result = new Response<string>();
            try
            {
                string swiftFileText = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
                result.Data = swiftFileText;
                
                return result;
            }
            catch (Exception)
            {
                result.IsSuccessful = false;
                result.Message = ServicesConstants.ReadingFileErrorMessage;
                
                return result;
            }
        }

        private async Task<Response<Swift>> ParseSwiftBlocksAsync(string swiftFileText, Swift swiftBlocks)
        {
            var result = new Response<Swift>();
            var blockPattern = ServicesConstants.BlockPattern;
            var blockMatches = Regex.Matches(swiftFileText, blockPattern);

            if (!blockMatches.Any())
            {
                result.IsSuccessful = false;
                result.Message = ServicesConstants.MatchErrorMessage;
                return result;
            }

            foreach (Match match in blockMatches)
            {
                var blockNumber = match.Groups[ServicesConstants.BlockNumber].Value;
                var blockContent = match.Groups[ServicesConstants.BlockContent].Value;

                switch (blockNumber)
                {
                    case ServicesConstants.BlockNumber1:
                        swiftBlocks.BasicHeaderBlock = blockContent;
                        break;
                    case ServicesConstants.BlockNumber2:
                        swiftBlocks.ApplicationHeaderBlock = blockContent;
                        break;
                    case ServicesConstants.BlockNumber3:
                        swiftBlocks.UserHeaderBlock = blockContent;
                        break;
                }
            }
            result.Data = swiftBlocks;
            
            return result;
        }

        private async Task<Response<Swift>> ParseSwiftTextBlockAsync(string swiftFileText, Swift swift)
        {
            var result = new Response<Swift>();
            var textBlockPattern = ServicesConstants.TextBlockPattern;
            var textBlockMathes = Regex.Matches(swiftFileText, textBlockPattern);

            if (!textBlockMathes.Any())
            {
                result.IsSuccessful = false;
                result.Message = ServicesConstants.MatchErrorMessage;
                return result;
            }

            foreach (Match match in textBlockMathes)
            {
                string[] parts = match.Value.Split(':');
                if (parts.Length >= 2)
                {
                    string fieldCode = parts[1];
                    string fieldValue = parts[2];
                    fieldValue = fieldValue.Replace(Environment.NewLine, "");

                    switch (fieldCode)
                    {
                        case ServicesConstants.FieldCode20:
                            swift.TransactionReferenceNumber = fieldValue;
                            break;
                        case ServicesConstants.fieldCode21:
                            swift.RelatedReference = fieldValue;
                            break;
                    }
                }
            }
            result.Data = swift;
            
            return result;
        }

        private async Task<Response<Swift>> ParseSwiftNarrativeAsync(string fileText, Swift swift)
        {
            var result = new Response<Swift>();
            var lines = fileText.Split('\n');
            if (!lines.Any())
            {
                result.IsSuccessful = false;
                result.Message = ServicesConstants.MatchErrorMessage;
                return result;
            }

            var extractedText = "";
            var insideBlock = false;

            foreach (string line in lines)
            {
                if (insideBlock)
                {
                    if (line.StartsWith(ServicesConstants.TextBlockFinishWith))
                    {
                        break;
                    }
                    extractedText += line + "\n";
                }
                else if (line.StartsWith(ServicesConstants.TextBlockStartsWith))
                {
                    insideBlock = true;
                    extractedText = line.Substring(4) + Environment.NewLine;
                }
            }
            swift.Narrative = extractedText;
            result.Data = swift;
            
            return result;
        }

        private async Task<Response<Swift>> ParseTrailerBlockAsync(string fileText, Swift swift)
        {
            var result = new Response<Swift>();
            string trailerBlockPattern = ServicesConstants.TrailerBlockPattern;

            Match match = Regex.Match(fileText, trailerBlockPattern);

            if (!match.Success)
            {
                result.IsSuccessful = false;
                result.Message = ServicesConstants.MatchErrorMessage;
            }
            result.Data = swift;
            result.Data.TrailerBlockMac = match.Groups[1].Value;
            result.Data.TrailerBlockChk = match.Groups[2].Value;

            return result;
        }
    }
}