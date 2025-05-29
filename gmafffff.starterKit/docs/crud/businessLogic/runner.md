# Обработка бизнес-логики

## Схема работы

Механизм обработки бизнес-команд
`gmafffff.starterKit.BusinessLogic.BusinessActionRunner<TCommand>`
автоматически запускает:

* Валидацию `Validot.ISpecificationHolder<TValidableEntity>`.
* Проверку бизнес-правил `gmafffff.starterKit.BusinessLogic.IBusinessRule`.
* Запуск обработчика команды `gmafffff.starterKit.BusinessLogic.IBusinessCommandHandler`.
* Запуск обработчиков доменных событий `gmafffff.starterKit.Domain.Events.DomainEvent<T>`
* Запуска обработчиков команд в ответ на сигнальные события `gmafffff.starterKit.Messaging.TriggerEvent`.
* Перехват исключений `OperationCanceledException` и `DbUpdateConcurrencyException`
для преобразования их в монаду `LanguageExt.Fin`

![Принципиальная схема обработки команды](https://www.plantuml.com/plantuml/svg/XLAzRXGn4Exz56PfxXHTGubGY1AIae94LDTOUyF5OjTU_7C1Adv4TUy0hv0Wa222unMyRyItzjgvfU6AlVdvvflnV7DadJ3EjmsxrncnRWLtfDzoPpmn9cgd3Jzf92d7XULrj5Y72bdBJY3wp2j5PaWQhrYjbJEwuTM54SgYN6grLDpMkgDg03d49_NFHIDNmkb8iHaSa3GDnLBBgsDlfI9hbwg6ZtAfd8rKoP-GLIUwROLQBTKblWsP21njXj6bUyEhq_WVKmYnLvuy8Rg1irJdKhsCAGjZIlv4j-ZEAG5NqdR2rPTZa-nEff-KJ8UYqPdYBunShybmfx6ryXWtBBF1ab9fTcawtbEwMgeaHpNgyO2n91bV70u-FpZW3pczfjexcjM9VyuOKa14Oa3kdvqTumUaeYl6raRxZeSl_Rkm2r_3T_pVXkjS4oK3LDgevECSJ-h4nBbmqhwGX2IfLI7y76v0THrkmgx_C51oV7RX5qA_mxT-Ep9b-J3OOd4tFSjX84Wv9BmbbfuQjHkWhNHfz_leh1G0sKmWougl7oqETQGfCPGMqL2Qh5aidD_HvpsdMGqvRnG_ZUmS63uA_9z5zE_3d_uZlhV3oljj_sa8xO3R0l4Z_2oAvLD0isBFN6eoPoLK1YipuLhoWTdCDbpCtdr0_H4nJ_FkDGKCvzCrv8YjIE1-DsWJKuob5l--dns1nBVDNm00 "Принципиальная схема обработки команды")

## DI

Регистрация в контейнере DI осуществляется во время вызова
метода расширения `IServiceCollection.AddBusinessLogic`.
