function OnInit()
	indexload = 0
	size_table = 21 -- количество строк таблицы
tiker = "SiM6SiU6"
	per ="5"
	timeframe = "INTERVAL_M5"
	progname = "Script is working : "
end

function hhmmss(date_time)
	
	local Hour = date_time.hour
	if Hour<10 then Hour = "0"..Hour end
	
	local Min = date_time.min
	if Min<10 then Min = "0"..Min end
	
	local Sec=date_time.sec
	if Sec<10 then Sec="0"..Sec end
		
	return Hour..Min..Sec

end


function YYYYDDMM(date_time)
	
	local DD = date_time.day
	if DD<10 then DD = "0"..DD end
	
	local MM = date_time.month
	if MM<10 then MM = "0"..MM end
		
	local YYYY = date_time.year
		
	return YYYY..MM..DD

end

function main()
	
	message(progname.." Start script")
	

	ds, error_discr = CreateDataSource("FUTSPREAD", "SiM6SiU6" , INTERVAL_M5)
	repeat
		sleep(100)
		indexload = indexload + 1
	until(ds:Size()~=0 or indexload>=10)

	
	number_of_candles = ds:Size()

	message("Number of candles "..number_of_candles)

	
	
	DirectionSaveFile=tostring("C:\\ATON\\1.csv")
	--создаем файл для записи
	my_csv=io.open(DirectionSaveFile,"w")

	
	

	if table_result==nil then  
	
		table_result = AllocTable() 
		AddColumn(table_result, 1, "<DATA>", true, QTABLE_DATE_TYPE, 12) 
		AddColumn(table_result, 2, "<TIME>", true, QTABLE_TIME_TYPE, 10) -- QTABLE_STRING_TYPE
		AddColumn(table_result, 3, "<OPEN>", true, QTABLE_DOUBLE_TYPE, 8) 
		AddColumn(table_result, 4, "<HIGH>", true, QTABLE_DOUBLE_TYPE, 8) 
		AddColumn(table_result, 5, "<LOW>", true, QTABLE_DOUBLE_TYPE, 8) 
		AddColumn(table_result, 6, "<CLOSE>", true, QTABLE_DOUBLE_TYPE, 9) 
		AddColumn(table_result, 7, "<VOLUME>", true, QTABLE_INT_TYPE, 15) 
		CreateWindow(table_result) 
		SetWindowPos(table_result,0,440,500,420) 
		SetWindowCaption(table_result, "Выгрузка котировок : "..tiker.." таймфрейм : "..timeframe)
		
		for u = 1, size_table do 
			InsertRow(table_result,-1)	
		end
	end
	
	my_csv:write("Ticker,Period,Date,Time,Open,High,Low,Close,Volume\n")
	
	for index = 1, number_of_candles  do
	x = tiker
	y = per
		openprice 	= ds:O(index)
		highprice 	= ds:H(index)
		lowprice 	= ds:L(index)
		closeprice 	= ds:C(index)
		volume 		= ds:V(index)
		localtime 	= hhmmss(ds:T(index))
		localdata 	= YYYYDDMM(ds:T(index))
		
		my_csv:write(x..","..y..","..localdata..","..localtime..","..openprice..","..highprice..","..lowprice..","..closeprice..","..volume.."\n")
		
		--наполнение таблицы
		if index >= number_of_candles-9 and  index <= (number_of_candles) then
			local linetable = 1 + number_of_candles - index
			SetCell(table_result, linetable, 1, tostring(localdata))
			SetCell(table_result, linetable, 2, tostring(localtime))
			SetCell(table_result, linetable, 3, tostring(openprice))
			SetCell(table_result, linetable, 4, tostring(highprice))
			SetCell(table_result, linetable, 5, tostring(lowprice))
			SetCell(table_result, linetable, 6, tostring(closeprice))
			SetCell(table_result, linetable, 7, tostring(math.floor(volume)))
		end
		
		for i = 1, 7 do
			SetCell(table_result, 11, i, "...")
		end
		
		if index >= 1 and  index <= 10 then
			local linetable = 22 - index
			SetCell(table_result, linetable, 1, tostring(localdata))
			SetCell(table_result, linetable, 2, tostring(localtime))
			SetCell(table_result, linetable, 3, tostring(openprice))
			SetCell(table_result, linetable, 4, tostring(highprice))
			SetCell(table_result, linetable, 5, tostring(lowprice))
			SetCell(table_result, linetable, 6, tostring(closeprice))
			SetCell(table_result, linetable, 7, tostring(math.floor(volume)))
		end
		
	end
	
	-- сохраняем и закрываем файл
	my_csv:flush() 
	my_csv:close()
	
	
	ds:Close() -- закрыть источник данных
	
	message(progname.." Finish.")
	
end