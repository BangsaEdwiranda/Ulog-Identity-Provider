﻿using ComponentSpace.Saml2.Configuration;
using ComponentSpace.Saml2.Configuration.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace CreateConfiguration
{
    /// <summary>
    /// Creates local identity provider or service provider SAML configuration.
    /// 
    /// Usage: dotnet CreateConfiguration.dll
    /// </summary>
    class Program
    {
        static void Main()
        {
            try
            {
                var samlConfigurations = new SamlConfigurations()
                {
                    Configurations = new List<SamlConfiguration>()
                    {
                        new SamlConfiguration()
                    }
                };

                Console.Write("Create Identity Provider or Service Provider configuration (IdP | SP): ");

                switch (ReadLine()?.ToLower())
                {
                    case "idp":
                        samlConfigurations.Configurations[0].LocalIdentityProviderConfiguration = CreateIdentityProviderConfiguration();
                        break;

                    case "sp":
                        samlConfigurations.Configurations[0].LocalServiceProviderConfiguration = CreateServiceProviderConfiguration();
                        break;

                    default:
                        throw new ArgumentException("The provider type must either be \"IdP\" or \"SP\".");
                }

                SaveConfiguration(samlConfigurations);
            }

            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        private static LocalIdentityProviderConfiguration CreateIdentityProviderConfiguration()
        {
            var localIdentityProviderConfiguration = new LocalIdentityProviderConfiguration()
            {
                Name = GetProviderName()
            };

            Console.Write("Single Sign-On Service URL [None]: ");
            localIdentityProviderConfiguration.SingleSignOnServiceUrl = ReadLine();

            Console.Write("Single Logout Service URL [None]: ");
            localIdentityProviderConfiguration.SingleLogoutServiceUrl = ReadLine();

            localIdentityProviderConfiguration.LocalCertificates = GetCertificates();

            return localIdentityProviderConfiguration;
        }

        private static LocalServiceProviderConfiguration CreateServiceProviderConfiguration()
        {
            var localServiceProviderConfiguration = new LocalServiceProviderConfiguration()
            {
                Name = GetProviderName()
            };

            Console.Write("Assertion Consumer Service URL [None]: ");
            localServiceProviderConfiguration.AssertionConsumerServiceUrl = ReadLine();

            Console.Write("Single Logout Service URL [None]: ");
            localServiceProviderConfiguration.SingleLogoutServiceUrl = ReadLine();

            localServiceProviderConfiguration.LocalCertificates = GetCertificates();

            return localServiceProviderConfiguration;
        }

        private static string GetProviderName()
        {
            Console.Write("Name: ");
            var name = ReadLine();

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("A provider name must be specified.");
            }

            return name;
        }

        private static IList<Certificate> GetCertificates()
        {
            Console.Write("X.509 certificate PFX file [None]: ");
            var fileName = ReadLine();

            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            Console.Write("X.509 certificate PFX password [None]: ");
            var password = ReadLine();

            return new List<Certificate>()
            {
                new Certificate()
                {
                    FileName = fileName,
                    Password = password
                }
            };
        }

        private static void SaveConfiguration(SamlConfigurations samlConfigurations)
        {
            Console.Write("SAML configuration file [saml.json]: ");

            var fileName = ReadLine();

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "saml.json";
            }

            File.WriteAllText(fileName, ConfigurationSerializer.Serialize(samlConfigurations));
        }

        private static string ReadLine()
        {
            var line = Console.ReadLine();

            if (string.IsNullOrEmpty(line))
            {
                line = null;
            }

            return line;
        }
    }
}
