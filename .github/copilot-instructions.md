# Copilot Instructions

## General Guidelines
- User prefers PowerShell as their terminal and workspace root is C:\Users\RUBEN\Desktop\CrediAvanzaAPI

## Project-Specific Rules
- El cálculo del IGV/IVA en los calendarios de pago (amortización) debe realizarse únicamente sobre el componente de capital, no sobre los intereses.
- Para el cálculo de calendarios de créditos mensuales (NSubProd = 4 o 1), el interés se calcula con la fórmula: (tasa * 12.0 / 365) / 100.0 * 30.