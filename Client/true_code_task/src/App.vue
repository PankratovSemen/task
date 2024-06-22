

<template>

<div class="container">
    <div class="row">
        <div class="col-12">
            <p>Вставьте ссылку на страницу или файл</p>
            <input type="text"  v-model="link" class="col-10">
            <button class="btn-secondary" @click="getFile">Получить</button>
        </div>
    </div>
</div>

</template>
<script>
import axios from "axios";


export default {
    data() {
        return {
            link:'',
            url:'https://localhost:7287/api/Generate/pdf?url='
        }
    },
    methods: {
        async getFile() {
            //Отправляем запрос
            const response = await axios({
                method:'GET',
                url:this.url + this.link,
                responseType:'blob'
            })
            //Если ответ успешный переходим далььше
            if (response.status === 200) {
                //Получаем данные из header
                const dispositionHeader = response.headers['content-disposition'];

                const regex = /filename=["']?([^'"s]+)["']?/;
                const matches = regex.exec(dispositionHeader);
                let filename = null;
                if (matches != null && matches.length > 1) {
                    //Находим имя скаченного файла
                    filename = matches[1];
                    console.log(filename);
                }
                const href = URL.createObjectURL(response.data);

                const link = document.createElement('a');
                try {
                    link.href = href;
                    //Установка аттрибутов для ссылки
                    link.setAttribute('download', filename);
                    document.body.appendChild(link);
                    link.click();
                }
                finally {
                    document.body.removeChild(link);
                    URL.revokeObjectURL(href);
                }
        }}}}
</script>
<style scoped>
button{
    background-color:blue;
    color:white;
    border-radius: 10px;
}
</style>
